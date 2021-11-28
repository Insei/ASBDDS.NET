using System;

namespace ASBDDS.Shared.Dtos.File
{
    public class SharedFileDto
    {
        public Guid Id { get; set; }
        public bool ShareViaHttp { get; set; }
        public bool ShareViaTftp { get; set; }
        public FileInfoModelDto File { get; set; }
    }
}