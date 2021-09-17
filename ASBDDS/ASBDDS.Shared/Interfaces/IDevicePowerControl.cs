using ASBDDS.Shared.Models.Database.DataDb;
using System.Threading;
using System.Threading.Tasks;

namespace ASBDDS.Shared.Interfaces
{
    public interface IDevicePowerControl
    {
        Task<int> PowerOn(Device device,  CancellationToken cancellationToken = default);
        Task<int> PowerOff(Device device, CancellationToken cancellationToken = default);
    }
}
