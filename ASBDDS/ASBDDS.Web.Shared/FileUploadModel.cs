using System.IO;

namespace ASBDDS.Web.Shared
{
    public class FileUploadModel
    {
        public string Name { get; set; }
        public Stream Stream { get; set; }
    }
}