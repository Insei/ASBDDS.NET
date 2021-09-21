using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Responses;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.API.Models;
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace ASBDDS.API.Controllers
{
    [ApiController]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly DevicePowerControlManager _devicePowerControl;

        public DevicesController(DataDbContext context, DevicePowerControlManager devicePowerControl)
        {
            _context = context;
            _devicePowerControl = devicePowerControl;
        }

        #region Admin panel API
        // GET: api/Devices
        /// <summary>
        /// Get all devices
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/admin/devices/")]
        public async Task<ActionResult<ApiResponse<List<DeviceAdminResponse>>>> GetDevices()
        {
            var resp = new ApiResponse<List<DeviceAdminResponse>>();
            try
            {
                var devices = await _context.Devices.Include(d => d.SwitchPort).ToArrayAsync();
                var _devices = new List<DeviceAdminResponse>();
                foreach(Device device in devices)
                {
                    _devices.Add(new DeviceAdminResponse(device));
                }
                resp.Data = _devices;
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // GET: api/Devices/5/
        /// <summary>
        /// Get device by ID
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <returns></returns>
        [HttpGet("api/admin/devices/{id}")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> GetDevice(Guid id)
        {
            var resp = new ApiResponse<DeviceAdminResponse>();
            try
            {
                var device = await _context.Devices.Where(d => d.Id == id).Include(d => d.SwitchPort).FirstOrDefaultAsync();
                if(device == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Device not found";
                    return resp;
                }
                else
                {
                    resp.Data = new DeviceAdminResponse(device);
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update device by ID
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <param name="deviceReq">See schema</param>
        /// <returns></returns>
        [HttpPut("api/admin/devices/{id}")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> PutDevice(Guid id, DeviceAdminPutRequest @deviceReq)
        {
            var resp = new ApiResponse<DeviceAdminResponse>();
            try
            {
                var device = await _context.Devices.Where(d => d.Id == id).Include(d => d.SwitchPort).ThenInclude(d => d.Switch).FirstOrDefaultAsync();

                if(device == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Device not found";
                    return resp;
                }

                var switchPort = await _context.SwitchPorts.FindAsync(deviceReq.SwitchPortId);

                if(switchPort == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch port not found";
                    return resp;
                }

                device.Manufacturer = deviceReq.Manufacturer;
                device.MacAddress = deviceReq.MacAddress;
                device.Model = deviceReq.Model;
                device.Name = deviceReq.Name;
                device.Serial = deviceReq.Serial;
                device.PowerControlType = deviceReq.PowerControlType;
                device.SwitchPort = switchPort;
                
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new DeviceAdminResponse(device);
                
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // POST: api/Devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Add new device
        /// </summary>
        /// <param name="deviceReq">See schema</param>
        /// <returns></returns>
        [HttpPost("api/admin/devices/")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> PostDevice(DeviceAdminPostRequest @deviceReq)
        {
            var resp = new ApiResponse<DeviceAdminResponse>();
            try
            {
                var _switchPort = await _context.SwitchPorts.FindAsync(deviceReq.SwitchPortId);
                if(_switchPort == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Switch port not found";
                    return resp;
                }
               
                var device = new Device
                {
                    Manufacturer = deviceReq.Manufacturer,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Serial = deviceReq.Serial,
                    StateEnum = DeviceState.POWEROFF,
                    SwitchPort = _switchPort,
                    MacAddress = deviceReq.MacAddress,
                    Model = deviceReq.Model,
                    Name = deviceReq.Name,
                    PowerControlType = deviceReq.PowerControlType
                };
                _context.Devices.Add(device);
                await _context.SaveChangesAsync();
                resp.Data = new DeviceAdminResponse(device);
                
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // DELETE: api/Devices/5
        /// <summary>
        /// Delete device by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/admin/devices/{id}")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> DeleteDevice(Guid id)
        {
            var resp = new ApiResponse<DeviceAdminResponse>();
            try
            {
                var device = await _context.Devices.Where(d => d.Id == id).Include(d => d.SwitchPort).FirstOrDefaultAsync();
                if (device == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Device not found";
                    return resp;
                }

                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
                resp.Data = new DeviceAdminResponse(device);
                
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        /// <summary>
        /// Poweroff the device by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/admin/devices/{id}/poweroff")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> PowerOffDevice(Guid id)
        {
            return await AdminPowerSwitchDevice(id, false);
        }

        /// <summary>
        /// Poweron the device by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/admin/devices/{id}/poweron")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> PowerOnDevice(Guid id)
        {
            return await AdminPowerSwitchDevice(id, true);
        }

        /// <summary>
        /// Reboot the device by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/admin/devices/{id}/reboot")]
        public async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> RebootDevice(Guid id)
        {
            await AdminPowerSwitchDevice(id, false);
            Thread.Sleep(1000);
            return await AdminPowerSwitchDevice(id, true);
        }

        private async Task<ActionResult<ApiResponse<DeviceAdminResponse>>> AdminPowerSwitchDevice(Guid id, bool enable)
        {
            var resp = new ApiResponse<DeviceAdminResponse>();
            try
            {
                var device = await _context.Devices.Where(d => d.Id == id).Include(d => d.SwitchPort).FirstOrDefaultAsync();
                if (device == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Device not found";
                    return resp;
                }

                if (enable)
                {
                    _devicePowerControl.SwitchPower(device, DevicePowerAction.PowerOn);
                    device.StateEnum = DeviceState.POWERON;
                }
                else
                {
                    _devicePowerControl.SwitchPower(device, DevicePowerAction.PowerOff);
                    device.StateEnum = DeviceState.POWEROFF;
                }

                _context.Entry(device).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                resp.Data = new DeviceAdminResponse(device);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        #endregion
    }
}
