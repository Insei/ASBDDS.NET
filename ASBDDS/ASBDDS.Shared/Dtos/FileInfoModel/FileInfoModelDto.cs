using System;

namespace ASBDDS.Shared.Dtos.File
{
    public class FileInfoModelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extension  { get; set; }
        public string FullName { get; set; }
    }
}