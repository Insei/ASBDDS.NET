using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.API.Models;
using ASBDDS.API.Models.Storage;
using ASBDDS.NET.Interfaces;
using ASBDDS.Shared.Dtos.File;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ASBDDS.API.Controllers
{
    [Route("api/files/")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storage;
        private readonly IMapper _mapper;
        private readonly DataDbContext _context;

        public FilesController(IStorageService storage, IMapper mapper, DataDbContext context)
        {
            _storage = storage;
            _mapper = mapper;
            _context = context;
        }

        private string GetFilename(IFormFile file)
        {
            return file.FileName.Substring(0, file.FileName.Length - (GetFileExtension(file).Length + 1));
        }

        private string GetFileExtension(IFormFile file)
        {
            return file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
        }
        
        /// <summary>
        /// Upload files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResponse<IList<FileInfoModelDto>>> Upload(IList<IFormFile> files)
        {
            var resp = new ApiResponse<IList<FileInfoModelDto>>();
            try
            {
                var uploadedFiles = new List<FileInfoModelDto>();
                foreach (var file in files)
                {
                    var storageFileInfo = new StorageFileInfoModel()
                    {
                        Name = GetFilename(file),
                        FullName = file.FileName,
                        Extension = GetFileExtension(file),
                        FileStream = file.OpenReadStream()
                    };
                    
                    try
                    {
                        _storage.Save(storageFileInfo);
                        _context.FileInfoModels.Add(storageFileInfo);
                        uploadedFiles.Add(_mapper.Map<FileInfoModelDto>(storageFileInfo));
                    }
                    catch
                    {
                        _storage.Delete(storageFileInfo);
                    }
                }
                resp.Data = uploadedFiles;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                resp.Status.Code = 1;
                resp.Status.Message = ex.Message;
            }

            return resp;
        }

        /// <summary>
        /// Delete files
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResponse> Delete([FromQuery]IList<Guid> ids)
        {
            var resp = new ApiResponse();
            try
            {
                var fileInfoModels = await _context.FileInfoModels.Where(f => ids.Contains(f.Id)).ToListAsync();
                var filesInUse = await _context.SharedOsFiles.Where(f => ids.Contains(f.FileId)).ToListAsync();
                if (filesInUse.Count > 0)
                {
                    resp.Status.Message = "several files are currently in use";
                    resp.Status.Code = 1;
                    return resp;
                }
                foreach (var fileInfoModel in fileInfoModels)
                {
                    _storage.Delete(fileInfoModel);
                }
                _context.FileInfoModels.RemoveRange(fileInfoModels);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                resp.Status.Code = 1;
                resp.Status.Message = ex.Message;
            }
        
            return resp;
        }
    }
}