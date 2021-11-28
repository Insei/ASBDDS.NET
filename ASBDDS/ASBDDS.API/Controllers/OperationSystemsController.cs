using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.Shared.Dtos.File;
using ASBDDS.Shared.Dtos.OperationSystem;
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
    public class OperationSystemsController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly IMapper _mapper;

        public OperationSystemsController(DataDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all operation systems
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/os/")]
        public async Task<ApiResponse<List<OperationSystemDto>>> GetOperationSystems()
        {
            var resp = new ApiResponse<List<OperationSystemDto>>();
            try
            {
                resp.Data = await _context.OperationSystemModels.
                    Where(os => !os.Disabled).
                    Select(os => _mapper.Map<OperationSystemDto>(os))
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
        /// Create new operations system
        /// </summary>
        /// <param name="operationSystemCreateDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("api/admin/os/")]
        public async Task<ApiResponse<OperationSystemDto>> CreateOperationSystem(OperationSystemCreateDto operationSystemCreateDto)
        {
            var resp = new ApiResponse<OperationSystemDto>();
            try
            {
                var newOs = new OperationSystemModel();
                _mapper.Map(operationSystemCreateDto, newOs);
                var alreadyExist = await _context.OperationSystemModels
                    .AnyAsync(os => 
                        os.Arch == newOs.Arch && 
                        os.Name == newOs.Name && 
                        os.Version == newOs.Version);
                if (alreadyExist)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "System already exist";
                }
                _context.OperationSystemModels.Add(newOs);
                await _context.SaveChangesAsync();

                resp.Data = _mapper.Map<OperationSystemDto>(newOs);
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Update OS
        /// </summary>
        /// <param name="id"></param>
        /// <param name="osUpdateDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("api/admin/os/{id}")]
        public async Task<ApiResponse<OperationSystemDto>> UpdateOperationSystem(Guid id, OperationSystemUpdateDto osUpdateDto)
        {
            var resp = new ApiResponse<OperationSystemDto>();
            try
            {
                var os = await _context.OperationSystemModels.FirstOrDefaultAsync(os => os.Id == id);
                if (os == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Operation system not found";
                    return resp;
                }
                _mapper.Map(osUpdateDto, os);
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
        /// Remove operation system
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("api/admin/os/{id}")]
        public async Task<ApiResponse> DeleteOperationSystem(Guid id)
        {
            var resp = new ApiResponse();
            try
            {
                var os = await _context.OperationSystemModels.FirstOrDefaultAsync(os => os.Id == id);
                if (os == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Operation system not found";
                    return resp;
                }
                var sharedOsFiles = await _context.SharedOsFiles.Where(f => f.Os == os).ToListAsync();
                _context.SharedOsFiles.RemoveRange(sharedOsFiles);
                os.Disabled = true;
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
        /// Get operation system files
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("api/admin/os/{id}/files/")]
        public async Task<ApiResponse<List<SharedFileDto>>> GetOperationSystemFiles(Guid id)
        {
            var resp = new ApiResponse<List<SharedFileDto>>();
            try
            {
                var os = await _context.OperationSystemModels.FirstOrDefaultAsync(os => os.Id == id && !os.Disabled);
                if (os == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Operation system not found";
                }
                
                var sharedOsFiles = await _context.SharedOsFiles.Where(f => f.Os == os).ToListAsync();
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
        /// Add Operation system file
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
                var os = await _context.OperationSystemModels.FirstOrDefaultAsync(os => os.Id == id && !os.Disabled);
                if (os == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Operation system not found";
                    return resp;
                }
                var fileInfoModel = await _context.FileInfoModels.FirstOrDefaultAsync(f => f.Id == fileToShareModel.FileId);
                if (fileInfoModel == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "File not found";
                    return resp;
                }
                var fileAlreadyShared = await _context.SharedOsFiles.AnyAsync(f => f.Id == fileToShareModel.FileId && f.Os == os);
                if (fileAlreadyShared)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "File already shared";
                    return resp;
                }
                
                var newSharedOsFile = new SharedOsFile();
                _mapper.Map(fileToShareModel, newSharedOsFile);
                newSharedOsFile.Os = os;
                _context.SharedOsFiles.Add(newSharedOsFile);
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
        /// Remove operation system file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sharedFileId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("api/admin/os/{id}/files/{sharedFileId}")]
        public async Task<ApiResponse> DeleteOperationSystemFile(Guid id, Guid sharedFileId)
        {
            var resp = new ApiResponse();
            try
            {
                var os = await _context.OperationSystemModels.FirstOrDefaultAsync(os => os.Id == id && !os.Disabled);
                if (os == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Operation system not found";
                    return resp;
                }
                var sharedOsFile = await _context.SharedOsFiles.FirstOrDefaultAsync(f => f.Id == sharedFileId);
                if (sharedOsFile == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "File not found";
                    return resp;
                }
                _context.SharedOsFiles.Remove(sharedOsFile);
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