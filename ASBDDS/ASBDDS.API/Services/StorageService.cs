using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.API.Models.Storage;
using ASBDDS.NET.Interfaces;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ASBDDS.NET.Services
{
    public class StorageService : IStorageService
    {
        private static readonly string _rootPath = Path.Combine(Environment.CurrentDirectory, "files");

        static StorageService()
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
        }

        public async void Save(StorageFileInfoModel storageFileModel)
        {
            var filePath = Path.Combine(_rootPath, storageFileModel.Id.ToString());
            await using Stream destFileStream = new FileStream(filePath, FileMode.Create);
            await storageFileModel.FileStream.CopyToAsync(destFileStream);
        }

        public async Task<Stream> Load(StorageFileInfoModel storageFileModel)
        {
            await using Stream fileStream = new FileStream(Path.Combine(_rootPath, storageFileModel.Id.ToString()), FileMode.Open, FileAccess.Read);
            return fileStream;
        }

        public async void Delete(FileInfoModel storageFileModel)
        {
            if(File.Exists(Path.Combine(_rootPath, storageFileModel.Id.ToString())))
                await Task.Run( () => File.Delete(Path.Combine(_rootPath, storageFileModel.Id.ToString())));
        }
    }
}