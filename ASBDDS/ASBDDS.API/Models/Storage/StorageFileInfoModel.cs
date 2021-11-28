using System.IO;
using ASBDDS.Shared.Models.Database.DataDb;

namespace ASBDDS.API.Models.Storage
{
    public class StorageFileInfoModel : FileInfoModel
    {
        public Stream FileStream { get; set; }
    }
}