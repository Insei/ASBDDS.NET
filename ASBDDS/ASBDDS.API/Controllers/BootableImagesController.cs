using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.Shared.Dtos.File;
using ASBDDS.Shared.Dtos.BootableImage;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASBDDS.API.Controllers
{
    [ApiController]
    [Authorize]
    public class BootableImagesController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly IMapper _mapper;

        public BootableImagesController(DataDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all bootable images
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/admin/bootable-images/")]
        public async Task<ApiResponse<List<BootableImageDto>>> GetOperationSystems()
        {
            var resp = new ApiResponse<List<BootableImageDto>>();
            try
            {
                resp.Data = await _context.BootableImages.
                    Where(os => !os.Disabled).
                    Select(os => _mapper.Map<BootableImageDto>(os))
                    .ToListAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Create new bootable image
        /// </summary>
        /// <param name="bootableImageCreateDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("api/admin/bootable-images/")]
        public async Task<ApiResponse<BootableImageDto>> CreateBootableImage(BootableImageCreateDto bootableImageCreateDto)
        {
            var resp = new ApiResponse<BootableImageDto>();
            try
            {
                var newBootableImage = new BootableImage();
                _mapper.Map(bootableImageCreateDto, newBootableImage);
                var alreadyExist = await _context.BootableImages
                    .AnyAsync(bi => 
                        bi.Arch == newBootableImage.Arch && 
                        bi.Name == newBootableImage.Name && 
                        bi.Version == newBootableImage.Version &&
                        bi.InProtocol == newBootableImage.InProtocol &&
                        bi.OutProtocol == newBootableImage.OutProtocol);
                if (alreadyExist)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Bootable image already exist";
                }
                _context.BootableImages.Add(newBootableImage);
                await _context.SaveChangesAsync();

                resp.Data = _mapper.Map<BootableImageDto>(newBootableImage);
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Update Bootable image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bootableImageUpdateDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("api/admin/bootable-images/{id}")]
        public async Task<ApiResponse<BootableImageDto>> UpdateBootableImage(Guid id, BootableImageUpdateDto bootableImageUpdateDto)
        {
            var resp = new ApiResponse<BootableImageDto>();
            try
            {
                var os = await _context.BootableImages.FirstOrDefaultAsync(os => os.Id == id);
                if (os == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Bootable image not found";
                    return resp;
                }
                _mapper.Map(bootableImageUpdateDto, os);
                _context.Entry(os).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        /// <summary>
        /// Remove bootable image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("api/admin/bootable-images/{id}")]
        public async Task<ApiResponse> DeleteBootableImage(Guid id)
        {
            var resp = new ApiResponse();
            try
            {
                var bi = await _context.BootableImages.FirstOrDefaultAsync(os => os.Id == id);
                if (bi == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Bootable image not found";
                    return resp;
                }
                var sharedOsFiles = await _context.SharedBootableImageFiles.Where(f => f.BootableImage == bi).ToListAsync();
                _context.SharedBootableImageFiles.RemoveRange(sharedOsFiles);
                bi.Disabled = true;
                _context.Entry(bi).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Get bootable image files
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("api/admin/bootable-images/{id}/files/")]
        public async Task<ApiResponse<List<SharedFileDto>>> GetBootableImageFiles(Guid id)
        {
            var resp = new ApiResponse<List<SharedFileDto>>();
            try
            {
                var bi = await _context.BootableImages.FirstOrDefaultAsync(os => os.Id == id && !os.Disabled);
                if (bi == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Bootable image not found";
                }
                
                var sharedOsFiles = await _context.SharedBootableImageFiles.Where(f => f.BootableImage == bi).ToListAsync();
                var filesIds = sharedOsFiles.Select(f => f.FileId);
                var fileInfoModels = await _context.FileInfoModels.Where(f => filesIds.Contains(f.Id)).ToListAsync();
                resp.Data = fileInfoModels.Select(f => new SharedFileDto()
                {
                    Id = sharedOsFiles.FirstOrDefault(shared => shared.FileId == f.Id).Id,
                    File = _mapper.Map<FileInfoModelDto>(f),
                    ShareViaTftp = sharedOsFiles.FirstOrDefault(shared => shared.FileId == f.Id).ShareViaTftp,
                    ShareViaHttp = sharedOsFiles.FirstOrDefault(shared => shared.FileId == f.Id).ShareViaHttp
                }).ToList();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Add bootable image file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileToShareModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("api/admin/os/{id}/files/")]
        public async Task<ApiResponse> AddOperationSystemFile(Guid id, SharedFileCreateDto fileToShareModel)
        {
            var resp = new ApiResponse();
            try
            {
                var bi = await _context.BootableImages.FirstOrDefaultAsync(os => os.Id == id && !os.Disabled);
                if (bi == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Bootable image not found";
                    return resp;
                }
                var fileInfoModel = await _context.FileInfoModels.FirstOrDefaultAsync(f => f.Id == fileToShareModel.FileId);
                if (fileInfoModel == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "File not found";
                    return resp;
                }
                var fileAlreadyShared = await _context.SharedBootableImageFiles.AnyAsync(f => f.Id == fileToShareModel.FileId && 
                    f.BootableImage == bi);
                if (fileAlreadyShared)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "File already shared";
                    return resp;
                }
                
                var newSharedOsFile = new SharedBootableImageFile();
                _mapper.Map(fileToShareModel, newSharedOsFile);
                newSharedOsFile.BootableImage = bi;
                _context.SharedBootableImageFiles.Add(newSharedOsFile);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Remove bootable image file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sharedFileId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("api/admin/bootable-images/{id}/files/{sharedFileId}")]
        public async Task<ApiResponse> DeleteOperationSystemFile(Guid id, Guid sharedFileId)
        {
            var resp = new ApiResponse();
            try
            {
                var bi = await _context.BootableImages.FirstOrDefaultAsync(os => os.Id == id && !os.Disabled);
                if (bi == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Bootable image not found";
                    return resp;
                }
                var sharedOsFile = await _context.SharedBootableImageFiles.FirstOrDefaultAsync(f => f.Id == sharedFileId);
                if (sharedOsFile == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "File not found";
                    return resp;
                }
                _context.SharedBootableImageFiles.Remove(sharedOsFile);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
    }
}