using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;
using ASBDDS.API.Models;
using ASBDDS.API.Models.Utils;
using GitHub.JPMikkers.DHCP;

namespace ASBDDS.API.Controllers
{
    [Route("api/ipxe/")]
    [ApiController]
    public class IPXEController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly DevicePowerControlManager _devicePowerControl;
        public IPXEController(DataDbContext context, DevicePowerControlManager devicePowerControl)
        {
            _context = context;
            _devicePowerControl = devicePowerControl;
        }

        private static readonly string ipxeCfgReboot = 
            "#!ipxe\n"+
            "reboot";

        private static readonly string ipxeCfgNormal = 
            "#!ipxe\n" +
            "echo Getting the current time from ntp...\n" +
            ":retry_ntp\n" +
            "ntp pool.ntp.org || goto retry_ntp\n" +
            "chain --autofree REPLACEURL";
        private static readonly string ipxeCfgPowerOff = 
            "#!ipxe\n" +
            "poweroff";
        
        private async Task<Stream> OnEraseComplete(Device device)
        {
            // Disable POE on port
            await _devicePowerControl.SwitchPower(device, DevicePowerAction.PowerOff);
            
            var deviceRent = _context.DeviceRents.FirstOrDefault(dr => dr.Device.Id == device.Id && dr.Status == DeviceRentStatus.CLOSING);
            if (deviceRent != null)
            {
                deviceRent.Status = DeviceRentStatus.CLOSED;
                deviceRent.Closed = DateTime.Now;
                _context.Entry(deviceRent).State = EntityState.Modified;
            }

            device.PowerState = DevicePowerState.PowerOff;
            device.MachineState = DeviceMachineState.Free;
            _context.Entry(device).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            BootloaderSetupHelper.RemoveDeviceDirectory(device);
            
            return new MemoryStream(Encoding.UTF8.GetBytes(ipxeCfgPowerOff));
        }

        private async Task<Stream> OnProvisionComplete(Device device)
        {
            device.MachineState = DeviceMachineState.Ready;
            
            BootloaderSetupHelper.RemoveDeviceDirectory(device);
            BootloaderSetupHelper.MakeFirmware(device);
            BootloaderSetupHelper.MakeUboot(device, "normal");
            
            _context.Entry(device).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return new MemoryStream(Encoding.UTF8.GetBytes(ipxeCfgReboot));
        }

        private Stream GetUserIpxeCfgStream(Device device)
        {
            Stream ipxeConfigStream = null;
            var deviceRent = _context.DeviceRents.Include(dr => dr.Device).FirstOrDefault(dr => dr.Device.Id == device.Id && dr.Status == DeviceRentStatus.ACTIVE);
            if (deviceRent != null)
            {
                ipxeConfigStream = new MemoryStream(Encoding.UTF8.GetBytes(ipxeCfgNormal.Replace("REPLACEURL", deviceRent.IpxeUrl)));
            }

            return ipxeConfigStream;
        }
        
        private async Task<Stream> OnCreateComplete(Device device)
        {
            device.MachineState = DeviceMachineState.Provisioning;
            _context.Entry(device).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            
            return GetUserIpxeCfgStream(device);
        }

        [HttpGet("{macAddress}/ipxe.efi.cfg")]
        public async Task<FileResult> GetIPXEConfig(string macAddress)
        {
            var macBytes = Utils.HexStringToBytes(macAddress);
            var macString = Utils.BytesToHexString(macBytes, "-");

            var device = _context.Devices
                .FirstOrDefault(d => d.MacAddress.Equals(macString));
            
            Stream ipxeConfigStream = null;
            if (device != null)
            {
                switch (device.MachineState)
                {
                    case DeviceMachineState.Creating:
                        ipxeConfigStream = await OnCreateComplete(device);
                        break;
                    case DeviceMachineState.Provisioning:
                        ipxeConfigStream = await OnProvisionComplete(device);
                        break;
                    case DeviceMachineState.IPXEOnly:
                        ipxeConfigStream = GetUserIpxeCfgStream(device);
                        break;
                    case DeviceMachineState.Erasing:
                        // power off device
                        ipxeConfigStream = await OnEraseComplete(device);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            ipxeConfigStream ??= new MemoryStream(Encoding.UTF8.GetBytes(ipxeCfgReboot));
            var fileStream =  File(ipxeConfigStream, "application/octet-stream", "ipxe.efi.cfg");
            return fileStream;
        }
    }
}
