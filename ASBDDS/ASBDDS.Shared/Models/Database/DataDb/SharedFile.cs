using System;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class SharedFile : DbBaseModel
    {
        public bool ShareViaHttp { get; set; }
        public bool ShareViaTftp { get; set; }
        public Guid FileId { get; set; }
    }
}