using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ASBDDS.Shared.Helpers;
using ASBDDS.Shared.Interfaces;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;

namespace ASBDDS.API.Models
{
    public class DevicePowerControlManager
    {
        private readonly DataDbContext _context;
        
        public DevicePowerControlManager(DataDbContext context)
        {
            _context = context;
        }

        public async Task<int> Reboot(Device device, CancellationToken cancellationToken = default)
        {
            var rtt = SwitchPower(device, false, cancellationToken).ContinueWith(tt => SwitchPower(device, true, cancellationToken), cancellationToken);
            var powerOffResult = await rtt;
            var powerOnResult = await powerOffResult;
            return powerOnResult;
        }
        public async Task<int> SwitchPower(Device device, bool enable, CancellationToken cancellationToken = default)
        {
            if (device.PowerControlType == DevicePowerControlType.PoeSwitch)
            {
                var deviceWithIncludes = await _context.Devices.Where(d => device.Id == d.Id)
                    .Include(d => d.SwitchPort)
                    .ThenInclude(sp => sp.Switch).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                IDevicePowerControl powerControl = null;
                if (deviceWithIncludes.SwitchPort.Switch.Model ==
                    SwitchHelper.GetModel(SwitchModels.UNIFI_SWITCH_US_24_250W))
                {
                    powerControl = new UniFiSwitch();
                }

                if (powerControl != null)
                {
                    if(enable)
                        return await powerControl?.PowerOn(deviceWithIncludes, cancellationToken);

                    return await powerControl?.PowerOff(deviceWithIncludes, cancellationToken);
                }
            }

            return 1;
        }
    }
}