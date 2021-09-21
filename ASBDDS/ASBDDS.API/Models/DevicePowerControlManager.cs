using System;
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
        
        public async Task<int> SwitchPower(Device device, DevicePowerAction action, CancellationToken cancellationToken = default)
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
                    switch (action)
                    {
                        case DevicePowerAction.PowerOff:
                            return await powerControl?.PowerOff(deviceWithIncludes, cancellationToken);
                            break;
                        case DevicePowerAction.PowerOn:
                            return await powerControl?.PowerOn(deviceWithIncludes, cancellationToken);
                            break;
                        case DevicePowerAction.Reboot:
                            var powerOffResult = await powerControl?.PowerOff(deviceWithIncludes, cancellationToken);
                            var powerOnResult = await powerControl?.PowerOn(deviceWithIncludes, cancellationToken);
                            if (powerOffResult == 0 && powerOnResult == 0)
                                return 0;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(action), action, null);
                    }
                    return 1;
                }
            }

            return 0;
        }
    }
}