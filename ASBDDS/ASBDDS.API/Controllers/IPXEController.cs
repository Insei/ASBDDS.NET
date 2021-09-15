using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ASBDDS.API.Models.Utils;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;
using ASBDDS.Shared.Helpers;
using ASBDDS.API.Models;
using ASBDDS.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ASBDDS.API.Controllers
{
    [Route("api/ipxe")]
    [ApiController]
    [Authorize]
    public class IPXEController : ControllerBase
    {
        private readonly DataDbContext _context;
        public IPXEController(DataDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get IPXE configuration file
        /// </summary>
        /// <param name="serial">Device serial number</param>
        /// <returns></returns>
        [HttpGet("{serial}/ipxe.efi.cfg")]
        public async Task<FileResult> GetIPXEConfig(string serial)
        {
            string path = Path.Combine(BootloaderSetupHelper.ImagesDirectory, "ipxe", "reboot-ipxe.efi.cfg");
            var shortserial = serial.Substring(8, 8);
            var device = _context.Devices.Where(d => d.Serial == serial || d.Serial == shortserial).Include(d => d.SwitchPort).ThenInclude(s => s.Switch).FirstOrDefault();
            if (device != null)
            {
                if (device.StateEnum == DeviceState.CREATING)
                {
                    path = Path.Combine(BootloaderSetupHelper.TftpDirectory, device.Serial, "ipxe.efi.cfg");
                    device.StateEnum = DeviceState.PROVISIONING;
                }
                else if (device.StateEnum == DeviceState.PROVISIONING)
                {
                    BootloaderSetupHelper.MakeFirmware(device);
                    BootloaderSetupHelper.MakeUboot(device);
                    device.StateEnum = DeviceState.POWERON;
                }
                else if (device.StateEnum == DeviceState.ERASING)
                {
                    var @switch = device.SwitchPort.Switch;
                    var switchHelper = new SwitchHelper();
                    IControlPOESwitchPort poeControl = null;

                    if (switchHelper.GetManufacturer(@switch.Manufacturer) == SwitchManufacturers.UBIQUITI)
                    {
                        poeControl = new UniFiSwitch();
                    }
                    // TODO: ERROR HANDLER
                    if (poeControl != null)
                        poeControl.DisablePOEPort(device.SwitchPort);

                    BootloaderSetupHelper.RemoveDeviceDirectory(device);
                    device.StateEnum = DeviceState.POWEROFF;
                }
                else
                {
                    //TODO: ERROR HANDLER
                }
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return File(new FileStream(path, FileMode.Open), "application/octet-stream", "ipxe.efi.cfg");
        }
    }
}
