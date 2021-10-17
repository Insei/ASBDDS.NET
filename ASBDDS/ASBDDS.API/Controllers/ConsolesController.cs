using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.API.Models;
using ASBDDS.Shared.Dtos.DbConsole;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASBDDS.API.Controllers
{   
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/consoles/")]
    public class ConsolesController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly IMapper _mapper;

        public ConsolesController(DataDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all consoles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<AdminDbConsoleDto>>> GetConsoles()
        {
            var resp = new ApiResponse<List<AdminDbConsoleDto>>();
            try
            {
                var consoles = await _context.Consoles.Include(c => c.SerialSettings).ToListAsync();
                resp.Data = consoles.Select(console => _mapper.Map<AdminDbConsoleDto>(console)).ToList();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        /// <summary>
        /// Get console by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<AdminDbConsoleDto>> GetConsole(Guid id)
        {
            var resp = new ApiResponse<AdminDbConsoleDto>();
            try
            {
                var console = await _context.Consoles.Include(c => c.SerialSettings).FirstOrDefaultAsync(c => c.Id == id);
                if (console == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Not found";
                    return resp;
                }
                resp.Data = _mapper.Map<AdminDbConsoleDto>(console);
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Create Console
        /// </summary>
        /// <param name="consoleCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<AdminDbConsoleDto>> PostConsole(AdminDbConsoleCreateDto consoleCreateDto)
        {
            var resp = new ApiResponse<AdminDbConsoleDto>();
            try
            {
                SerialPortSettings serialSettings = null;
                if (consoleCreateDto.SerialSettings != null)
                {
                    serialSettings = new SerialPortSettings();
                    _mapper.Map(consoleCreateDto.SerialSettings, serialSettings);
                    await _context.SerialPortsSettings.AddAsync(serialSettings);
                    await _context.SaveChangesAsync();
                } 
                else if (consoleCreateDto.Type == DbConsoleType.Serial)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "bad request, serial settings is empty";
                    return resp;
                }

                var console = new DbConsole() { SerialSettings = serialSettings};
                _mapper.Map(consoleCreateDto, console);
                _context.Consoles.Add(console);
                await _context.SaveChangesAsync();
                
                resp.Data = _mapper.Map<AdminDbConsoleDto>(console);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }
        
        /// <summary>
        /// Update console
        /// </summary>
        /// <param name="id"></param>
        /// <param name="consoleUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<AdminDbConsoleDto>> PutConsole(Guid id, AdminDbConsoleUpdateDto consoleUpdateDto)
        {
            var resp = new ApiResponse<AdminDbConsoleDto>();
            try
            {
                var console = await _context.Consoles.Include(c => c.SerialSettings).FirstOrDefaultAsync(c => c.Id == id);
                if (console == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Not found";
                    return resp;
                }
                if (consoleUpdateDto.Type == DbConsoleType.Serial && consoleUpdateDto.SerialSettings == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "bad request, serial settings is empty";
                    return resp;
                }
                _mapper.Map(consoleUpdateDto, console);
                await _context.SaveChangesAsync();
                resp.Data = _mapper.Map<AdminDbConsoleDto>(console);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteConsole(Guid id)
        {
            var resp = new ApiResponse();
            try
            {
                var console = await _context.Consoles.Include(c => c.SerialSettings).FirstOrDefaultAsync(c => c.Id == id);
                if (console == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Not found";
                    return resp;
                }

                if (console.SerialSettings != null)
                    _context.SerialPortsSettings.Remove(console.SerialSettings);
                
                _context.Consoles.Remove(console);
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