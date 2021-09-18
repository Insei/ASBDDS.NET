using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public enum DeviceRentStatus
    {
        ACTIVE,
        CLOSING,
        CLOSED
    }
    
    public class DeviceRent : DbBaseModel
    {
        public string Name { get; set; }
        public virtual Project Project { get; set; }
        public DateTime? Closed { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public virtual Device Device { get; set; }
        public string IpxeUrl { get; set; }
        public DeviceRentStatus Status { get; set; }
    }
}
