﻿using System;
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

        public DevicesController(DataDbContext context)
        {
            _context = context;
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
        /// <param name="deviceReq"></param>
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
        /// <param name="deviceReq"></param>
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
                    Name = deviceReq.Name
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
                var device = await _context.Devices.Where(d => d.Id == id).Include(d => d.SwitchPort).ThenInclude(s => s.Switch).FirstOrDefaultAsync();
                if (device == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Device not found";
                    return resp;
                }

                DeviceSwitchPower(device, enable);

                if (enable)
                    device.StateEnum = DeviceState.POWERON;
                else
                    device.StateEnum = DeviceState.POWEROFF;

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

        private void DeviceSwitchPower(Device device, bool enable)
        {
            if (device?.SwitchPort?.Switch != null)
            {
                var unifiSwitch = new UniFiSwitch();
                if (enable)
                    unifiSwitch.EnablePOEPort(device.SwitchPort);
                else
                    unifiSwitch.DisablePOEPort(device.SwitchPort);
            }
        }

        #endregion

        //#region User panel API
        //// GET: api/Devices
        //[HttpGet("api/devices/")]
        //public async Task<ActionResult<ApiResponse<List<DeviceUserResponse>>>> GetUserDevices(Guid projectId)
        //{
        //    var resp = new ApiResponse<List<DeviceUserResponse>>();
        //    try
        //    {
        //        var devices = await _context.Devices.Where(d => d.ExternalId != null && d.Project != null && d.Project.Id == projectId)
        //            .Include(d => d.SwitchPort).ToArrayAsync();
        //        var _devices = new List<DeviceUserResponse>();
        //        foreach (Device device in devices)
        //        {
        //            _devices.Add(new DeviceUserResponse(device));
        //        }
        //        resp.Data = _devices;
        //    }
        //    catch (Exception e)
        //    {
        //        resp.Status.Code = 1;
        //        resp.Status.Message = e.Message;
        //    }
        //    return resp;
        //}

        //// GET: api/Devices/5
        //[HttpGet("api/devices/{id}")]
        //public async Task<ActionResult<ApiResponse<DeviceUserResponse>>> GetUserDevice(Guid id, Guid projectId)
        //{
        //    var resp = new ApiResponse<DeviceUserResponse>();
        //    try
        //    {
        //        var device = await _context.Devices.Where(d => d.ExternalId != null && d.ExternalId == id && d.Project != null && d.Project.Id == projectId)
        //            .Include(d => d.SwitchPort).FirstOrDefaultAsync();
        //        if (device == null)
        //        {
        //            resp.Status.Code = 1;
        //            resp.Status.Message = "Device not found";
        //            return resp;
        //        }

        //        resp.Data = new DeviceUserResponse(device);
                
        //    }
        //    catch (Exception e)
        //    {
        //        resp.Status.Code = 1;
        //        resp.Status.Message = e.Message;
        //    }
        //    return resp;
        //}

        //[HttpPut("api/devices/{id}")]
        //public async Task<ActionResult<ApiResponse<DeviceUserResponse>>> PutDevice(Guid id, DeviceUserPutRequest @deviceReq)
        //{
        //    var resp = new ApiResponse<DeviceUserResponse>();
        //    try
        //    {
        //        var device = await _context.Devices.Where(d => d.ExternalId != null && d.ExternalId == id).FirstOrDefaultAsync();
        //        device.Name = deviceReq.Name;

        //        _context.Entry(device).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        resp.Data = new DeviceUserResponse(device);
        //    }
        //    catch (Exception e)
        //    {
        //        resp.Status.Code = 1;
        //        resp.Status.Message = e.Message;
        //    }
        //    return resp;
        //}

        //// POST: api/Devices
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost("api/devices/")]
        //public async Task<ActionResult<ApiResponse<DeviceUserResponse>>> PostUserDevice(DeviceUserPostRequest @deviceReq, Guid projectId)
        //{
        //    var resp = new ApiResponse<DeviceUserResponse>();
        //    try
        //    {
        //        var project = await _context.Projects.FindAsync(projectId);
        //        if(project == null)
        //        {
        //            resp.Status.Code = 1;
        //            resp.Status.Message = "Project not found";
        //            return resp;
        //        }
        //        var device = await _context.Devices.Where(d => d.ExternalId == null && d.Model == deviceReq.Model).FirstOrDefaultAsync();
        //        if(device == null)
        //        {
        //            resp.Status.Code = 1;
        //            resp.Status.Message = "There are no available devices with the specified model";
        //            return resp;
        //        }
  
        //        device.ExternalId = new Guid();
        //        device.Name = deviceReq.Name;
        //        device.Project = project;
        //        device.StateEnum = DeviceState.CREATING;
        //        await _context.SaveChangesAsync();
        //        resp.Data = new DeviceUserResponse(device);
        //    }
        //    catch (Exception e)
        //    {
        //        resp.Status.Code = 1;
        //        resp.Status.Message = e.Message;
        //    }
        //    return resp;
        //}

        //// DELETE: api/Devices/5
        //[HttpDelete("api/devices/{id}")]
        //public async Task<ActionResult<ApiResponse<DeviceUserResponse>>> DeleteUserDevice(Guid id, Guid projectId)
        //{
        //    var resp = new ApiResponse<DeviceUserResponse>();
        //    try
        //    {
        //        var project = await _context.Projects.FindAsync(projectId);
        //        if (project == null)
        //        {
        //            resp.Status.Code = 1;
        //            resp.Status.Message = "Project not found";
        //            return resp;
        //        }

        //        var device = await _context.Devices.FindAsync(id);
        //        if (device == null)
        //        {
        //            resp.Status.Code = 1;
        //            resp.Status.Message = "Device not found";
        //            return resp;
        //        }

        //        device.StateEnum = DeviceState.ERASING;
        //        await _context.SaveChangesAsync();
        //        resp.Data = new DeviceUserResponse(device);
                
        //    }
        //    catch (Exception e)
        //    {
        //        resp.Status.Code = 1;
        //        resp.Status.Message = e.Message;
        //    }

        //    return resp;
        //}
        //#endregion
    }
}
