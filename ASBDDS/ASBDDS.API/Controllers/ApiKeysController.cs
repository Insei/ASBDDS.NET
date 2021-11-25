using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.Shared.Dtos.UserApiKey;
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
    [Route("api/apikeys/")]
    public class ApiKeysController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly IMapper _mapper;

        public ApiKeysController(DataDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all user API keys
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<UserApiKeyDto>>> GetUserApiKeys()
        {
            var resp = new ApiResponse<List<UserApiKeyDto>>();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                resp.Data = await _context.UserApiKeys.Where(k => k.Creator == user)
                    .Select(apiKey => _mapper.Map<UserApiKeyDto>(apiKey)).ToListAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Get API key by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<UserApiKeyDto>> GetApiKey(Guid id)
        {
            var resp = new ApiResponse<UserApiKeyDto>();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                var userApiKey = await _context.UserApiKeys.FirstOrDefaultAsync(k=>k.Id == id && k.Creator == user);
                if (userApiKey == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Not found";
                    return resp;
                }
                resp.Data = _mapper.Map<UserApiKeyDto>(userApiKey);
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Create new user API key
        /// </summary>
        /// <param name="userApiKeyCreateDto">user API key create DTO</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<UserApiKeyDto>> CreateUserApiKey(UserApiKeyCreateDto userApiKeyCreateDto)
        {
            var resp = new ApiResponse<UserApiKeyDto>();
            try
            {
                var apiKey = new UserApiKey();
                _mapper.Map(userApiKeyCreateDto, apiKey);
                
                apiKey.Creator = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                _context.UserApiKeys.Add(apiKey);
                await _context.SaveChangesAsync();

                resp.Data = _mapper.Map<UserApiKeyDto>(apiKey);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Update user API Key
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userApiKeyDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<UserApiKeyDto>> PutUserApiKey(Guid id, UserApiKeyUpdateDto userApiKeyDto)
        {
            var resp = new ApiResponse<UserApiKeyDto>();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                var userApiKey = await _context.UserApiKeys.FirstOrDefaultAsync(k => k.Id == id && k.Creator == user);
                if (userApiKey == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Not found";
                    return resp;
                }

                _mapper.Map(userApiKeyDto, userApiKey);
                _context.Entry(userApiKey).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                resp.Data = _mapper.Map<UserApiKeyDto>(userApiKey);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        /// <summary>
        /// Delete User API Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteConsole(Guid id)
        {
            var resp = new ApiResponse();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                var userApiKey = await _context.UserApiKeys.FirstOrDefaultAsync(k => k.Id == id && k.Creator == user);
                if (userApiKey == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Not found";
                    return resp;
                }

                _context.UserApiKeys.Remove(userApiKey);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
    }
}