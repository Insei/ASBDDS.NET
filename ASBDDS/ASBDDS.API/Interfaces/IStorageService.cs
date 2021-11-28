using System.IO;
using System.Threading.Tasks;
using ASBDDS.API.Models.Storage;
using ASBDDS.Shared.Models.Database.DataDb;

namespace ASBDDS.NET.Interfaces
{
    public interface IStorageService
    {
        void Save(StorageFileInfoModel storageFileModel);
        Task<Stream> Load(StorageFileInfoModel storageFileModel);
        void Delete(FileInfoModel storageFileModel);
    }
}