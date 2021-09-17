using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Responses;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ASBDDS.API.Controllers
{
    [Route("api/admin/switches")]
    [ApiController]
    [Authorize]
    public class SwitchesController : ControllerBase
    {
        private readonly DataDbContext _context;

        public SwitchesController(DataDbContext context)
        {
            _context = context;
        }

        // GET: api/Switches
        /// <summary>
        /// Get all switches
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<SwitchAdminResponse>>>> GetSwitches()
        {
            var resp = new ApiResponse<List<SwitchAdminResponse>>();
            try
            {
                var switches = await _context.Switches.Include(s => s.Ports).ToListAsync();
                var _switches = new List<SwitchAdminResponse>();
                foreach (Switch _switch in switches)
                {
                    _switches.Add(new SwitchAdminResponse(_switch));
                }
                resp.Data = _switches;
            }
            catch
            {
                resp.Status.Code = 1;
            }
            return resp;
        }

        // GET: api/Switches/5
        /// <summary>
        /// Get switch by ID
        /// </summary>
        /// <param name="id">Switch ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<SwitchAdminResponse>> GetSwitch(Guid id)
        {
            var resp = new ApiResponse<SwitchAdminResponse>();
            try
            {
                var _switch = await _context.Switches.Where(s => s.Id == id).Include(s => s.Ports).FirstOrDefaultAsync();
                if(_switch == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch not found";
                    return resp;
                }

                resp.Data = new SwitchAdminResponse(_switch);
                
            } 
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // PUT: api/Switches/id
        /// <summary>
        /// Update switch by ID
        /// </summary>
        /// <param name="id">Switch ID</param>
        /// <param name="switchReq">See schema</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<SwitchAdminResponse>> PutSwitch(Guid id, SwitchAdminPutRequest @switchReq)
        {
            var resp = new ApiResponse<SwitchAdminResponse>();
            try
            {
                var _switch = _context.Switches.Where(s => s.Id == id).Include(s => s.Ports).FirstOrDefault();

                if(_switch == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch not found";
                    return resp;
                }
                

                var model = SwitchHelper.GetModel(switchReq.Model);
                var manufacturer = SwitchHelper.GetManufacturer(switchReq.Manufacturer);
                if (model == null || manufacturer == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch is not supported";
                    return resp;
                }

                _switch.Name = switchReq.Name;
                _switch.Serial = switchReq.Serial;
                _switch.Ip = switchReq.Ip;
                _switch.AccessProtocol = switchReq.AccessProtocol;
                _switch.AuthMethod = switchReq.AuthMethod;
                _switch.Username = switchReq.Username;
                if (switchReq.Password != "**********" && !String.IsNullOrEmpty(switchReq.Password))
                    _switch.Password = switchReq.Password;
                _switch.Model = switchReq.Model;
                _switch.Manufacturer = switchReq.Manufacturer;

                // Completed
                // 1. Найти каждый порт, если указан айди и изменить его
                // 2. Если передан порт без айди - создать его
                // 3. Если в запросе нет порта с айди, который есть в базе, его надо удалить, если к нему не подключено устройств. Если устройства есть, выдать ошибку

                var currentPorts = _switch.Ports;
                _switch.Ports = new List<SwitchPort>();
                foreach (var reqPort in switchReq.Ports)
                {
                    if (reqPort.Id != null)
                    {
                        var _port = await _context.SwitchPorts.FindAsync(reqPort.Id);
                        _port.Number = reqPort.Number;
                        _port.Type = reqPort.Type;
                        _switch.Ports.Add(_port);
                    }
                    else
                    {
                        _switch.Ports.Add(new SwitchPort() { Number = reqPort.Number, Type = reqPort.Type, Switch = _switch, Id = new Guid() });
                    }
                }

                foreach (var port in currentPorts)
                {
                    if(!switchReq.Ports.Any(p => p.Id != null && p.Id == port.Id))
                    {
                        var hasDevice = _context.Devices.Where(d => d.SwitchPort.Id == port.Id).Any();
                        if(hasDevice)
                        {
                            resp.Status.Code = 1;
                            resp.Status.Message = "No port with attached devices is specified";
                            return resp;
                        }
                        _context.SwitchPorts.Remove(port);
                    }
                }

                _context.Entry(_switch).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new SwitchAdminResponse(_switch);
                
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // POST: api/Switches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Add new switch
        /// </summary>
        /// <param name="switchReq">See schema</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<SwitchAdminResponse>> PostSwitch(SwitchAdminPostRequest @switchReq)
        {
            var resp = new ApiResponse<SwitchAdminResponse>();
            try
            {
                var model = SwitchHelper.GetModel(switchReq.Model);
                var manufacturer = SwitchHelper.GetManufacturer(switchReq.Manufacturer);
                if (model == null || manufacturer == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch is not supported";
                    return resp;
                }

                var _switch = new Switch()
                {
                    Serial = @switchReq.Serial,
                    Name = @switchReq.Name,
                    Ip = switchReq.Ip,
                    Model = switchReq.Model,
                    Manufacturer = switchReq.Manufacturer,
                    Username = switchReq.Username,
                    Password = switchReq.Password,
                    AccessProtocol = switchReq.AccessProtocol,
                    AuthMethod = switchReq.AuthMethod
                };

                _switch.Ports = new List<SwitchPort>();
                foreach (var port in @switchReq.Ports)
                    _switch.Ports.Add(new SwitchPort() { Number = port.Number, Type = port.Type, Switch = _switch });
                _context.Switches.Add(_switch);
                await _context.SaveChangesAsync();
                resp.Data = new SwitchAdminResponse(_switch);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        // DELETE: api/Switches/5
        /// <summary>
        /// Delete switch
        /// </summary>
        /// <param name="id">Switch ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<SwitchAdminResponse>> DeleteSwitch(Guid id)
        {
            var resp = new ApiResponse<SwitchAdminResponse>();
            try
            {
                var _switch = await _context.Switches.Where(s => s.Id == id).Include(s => s.Ports).FirstOrDefaultAsync();
                if (_switch == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch not found";
                    return resp;
                }
                // Completed
                // Проверять, привязаны ли устройства, если да - ошибка
                foreach(var port in _switch.Ports)
                {
                    var hasDevice = _context.Devices.Where(d => d.SwitchPort.Id == port.Id).Any();
                    if (hasDevice)
                    {
                        resp.Status.Code = 1;
                        resp.Status.Message = "At least one port has a device bound to it";
                        return resp;
                    }
                    _context.SwitchPorts.Remove(port);
                }
                    
                
                _context.Switches.Remove(_switch);
                await _context.SaveChangesAsync();

                resp.Data = new SwitchAdminResponse(_switch);
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
