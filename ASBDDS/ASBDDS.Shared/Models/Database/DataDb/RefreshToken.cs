using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class RefreshToken : DbBaseModel
    {
        public virtual ApplicationUser User { get; set; }
        public string Token { get; set; }
        public string RefreshT { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
