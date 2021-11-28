using System;

namespace ASBDDS.Shared.Dtos.File
{
    public class SharedFileCreateDto
    {
        public Guid FileId { get; set; }
        public bool ShareViaHttp { get; set; }
        public bool ShareViaTftp { get; set; }
    }
}