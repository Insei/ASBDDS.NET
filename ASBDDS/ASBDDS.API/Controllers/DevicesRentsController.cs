﻿using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASBDDS.API.Controllers
{
    [ApiController]
    public class DevicesRentsController : ControllerBase
    {
        private readonly DataDbContext _context;

        public DevicesRentsController(DataDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/devicesrents")]
        public async Task<ActionResult<ApiResponse<List<DeviceRentUserResponse>>>> GetUserDevicesRents()
        {
            var resp = new ApiResponse<List<DeviceRentUserResponse>>();
            try
            {
                var devicesRents = await _context.DeviceRents.Include(d => d.Device).Include(d => d.Project).ToListAsync();
                var _devicesRents = new List<DeviceRentUserResponse>();
                foreach (var devRent in devicesRents)
                {
                    _devicesRents.Add(new DeviceRentUserResponse(devRent));
                }
                resp.Data = _devicesRents;
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        [HttpGet("api/devicesrents/{id}")]
        public async Task<ActionResult<ApiResponse<DeviceRentUserResponse>>> GetUserDeviceRent(Guid id)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var deviceRent = await _context.DeviceRents.FindAsync(id);
                if(deviceRent == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "device rent not found";
                    return resp;
                }
                resp.Data = new DeviceRentUserResponse(deviceRent);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        [HttpPut("api/devicesrents/{id}")]
        public async Task<ActionResult<ApiResponse<DeviceRentUserResponse>>> UpdateUserDeviceRent(Guid id, DeviceRentUserPutRequest devRentReq)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var deviceRent = await _context.DeviceRents.Include(r => r.Project ).Include(r => r.Device).Where(r => r.Id == id).FirstOrDefaultAsync();
                if (deviceRent == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "device rent not found";
                    return resp;
                }

                deviceRent.Name = devRentReq.Name;
                _context.Entry(deviceRent).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                resp.Data = new DeviceRentUserResponse(deviceRent);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        [HttpPost("api/devicesrents/")]
        public async Task<ActionResult<ApiResponse<DeviceRentUserResponse>>> CreateUserDeviceRent(DeviceRentUserPostRequest devRentReq)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var devicesIdinRents = await _context.DeviceRents.Where(r => r.Closed == null).Select(r => r.Device.Id).ToListAsync();
                var freeDevice = await _context.Devices
                    .Where(d => !devicesIdinRents.Contains(d.Id) && d.Manufacturer == devRentReq.Manufacturer && d.Model == devRentReq.Model)
                    .FirstOrDefaultAsync();

                if (freeDevice == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "there are no available devices of the requested model";
                    return resp;
                }

                var project = await _context.Projects.FindAsync(devRentReq.ProjectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                var deviceRent = new DeviceRent
                {
                    Name = devRentReq.Name,
                    Device = freeDevice,
                    Project = project,
                    IPXEUrl = devRentReq.IPXEUrl,
                    UserId = devRentReq.UserId
                };

                _context.DeviceRents.Add(deviceRent);
                await _context.SaveChangesAsync();

                resp.Data = new DeviceRentUserResponse(deviceRent);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        [HttpDelete("api/devicesrents/{id}")]
        public async Task<ActionResult<ApiResponse<DeviceRentUserResponse>>> DeleteUserDeviceRent(Guid id)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var deviceRent = await _context.DeviceRents.Include(r => r.Project).Include(r => r.Device).Where(r => r.Id == id).FirstOrDefaultAsync();
                if (deviceRent == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "device rent not found";
                    return resp;
                }

                deviceRent.Closed = DateTime.Now;
                await _context.SaveChangesAsync();

                resp.Data = new DeviceRentUserResponse(deviceRent);
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
