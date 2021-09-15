using ASBDDS.Shared.Models.Database.DataDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASBDDS.Shared.Interfaces
{
    public interface IControlPOESwitchPort
    {
        void EnablePOEPort(SwitchPort port);
        void DisablePOEPort(SwitchPort port);
    }
}
