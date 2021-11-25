using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class UserApiKey : DbBaseModel
    {
        public string Description { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public Guid Key { get; set; }

        public UserApiKey()
        {
            Key = Guid.NewGuid();
        }
    }
}