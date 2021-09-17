using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ASBDDS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using ASBDDS.API.Models.Utils;
using ASBDDS.Shared.Helpers;
using ASBDDS.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ASBDDS.API.Controllers
{
    
    [ApiController]
    [Authorize]
    [Route("api/devices/rents")]
    public class DevicesRentsController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly DevicePowerControlManager _devicePowerControl;
        public DevicesRentsController(DataDbContext context, DevicePowerControlManager devicePowerControl)
        {
            _context = context;
            _devicePowerControl = devicePowerControl;
        }

        /// <summary>
        /// Get all devices rents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<DeviceRentUserResponse>>> GetUserDevicesRents([FromHeader(Name="ProjectId")][Required] Guid projectId)
        {
            var resp = new ApiResponse<List<DeviceRentUserResponse>>();
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if(project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }

                var devicesRents = await _context.DeviceRents.Where(dr => dr.Project == project && dr.Closed == null)
                    .Include(d => d.Device)
                    .Include(d => d.Project)
                    .Include(d => d.Creator)
                    .ToListAsync();
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

        /// <summary>
        /// Get device rent by ID
        /// </summary>
        /// <param name="id">device rent id</param>
        /// <param name="projectId">project id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<DeviceRentUserResponse>> GetUserDeviceRent([FromHeader(Name="ProjectId")][Required] Guid projectId, Guid id)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }
                var deviceRent = await _context.DeviceRents.Where(dr => dr.Project == project && dr.Closed == null && dr.Id == id).FirstOrDefaultAsync();
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

        /// <summary>
        /// Update device rent
        /// </summary>
        /// <param name="id">Device rent ID</param>
        /// <param name="devRentReq">See schema</param>
        /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<DeviceRentUserResponse>> UpdateUserDeviceRent(Guid id, DeviceRentUserPutRequest devRentReq,
                                                                                        [FromHeader(Name="ProjectId")][Required] Guid projectId)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }
                var deviceRent = await _context.DeviceRents.Where(r => r.Id == id && r.Project == project && r.Closed == null)
                    .Include(r => r.Project).Include(r => r.Device).FirstOrDefaultAsync();
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

        /// <summary>
        /// Add new device rent
        /// </summary>
        /// <param name="devRentReq">See schema</param>
        /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<DeviceRentUserResponse>> CreateUserDeviceRent([FromHeader(Name="ProjectId")][Required] Guid projectId, 
                                                                                                        DeviceRentUserPostRequest devRentReq)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }
                var devicesIdInRents = await _context.DeviceRents.Where(r => r.Closed == null).Select(r => r.Device.Id).ToListAsync();
                var freeDevice = await _context.Devices
                    .FirstOrDefaultAsync(d => !devicesIdInRents.Contains(d.Id) && d.Manufacturer == devRentReq.Manufacturer && d.Model == devRentReq.Model);
                // TODO: advanced reasons if device can't be available for this request.
                // example: we have not device with selected model in pool. 
                if (freeDevice == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "there are no available devices of the requested model";
                    return resp;
                }

                var deviceRent = new DeviceRent
                {
                    Name = devRentReq.Name,
                    Device = freeDevice,
                    Project = project,
                    IpxeUrl = devRentReq.IPXEUrl,
                    Creator = user
                };
                
                // Setup bootloaders in TFTP folder
                BootloaderSetupHelper.MakeFirmware(deviceRent.Device);
                BootloaderSetupHelper.MakeUboot(deviceRent.Device, "provisioning");
                BootloaderSetupHelper.MakeIpxe(deviceRent.Device);
                deviceRent.Device.StateEnum = DeviceState.CREATING;

                // Enable POE on port
                await _devicePowerControl.SwitchPower(deviceRent.Device, true);
                
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

        /// <summary>
        /// Delete device rent by ID
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<DeviceRentUserResponse>> CloseUserDeviceRent([FromHeader(Name="ProjectId")][Required] Guid projectId, 
                                                                                                        Guid id)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }
                var deviceRent = await _context.DeviceRents.Include(dr => dr.Device).FirstOrDefaultAsync(r => r.Id == id && r.Closed == null && r.Project == project);
                if (deviceRent == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "device rent not found";
                    return resp;
                }

                deviceRent.Status = DeviceRentStatus.CLOSING;
                
                BootloaderSetupHelper.RemoveDeviceDirectory(deviceRent.Device);
                BootloaderSetupHelper.MakeFirmware(deviceRent.Device);
                BootloaderSetupHelper.MakeUboot(deviceRent.Device, "erase-sdcard");
                BootloaderSetupHelper.MakeIpxe(deviceRent.Device);
                
                // Reboot device via POE on port
                var device = deviceRent.Device;
                await _devicePowerControl.SwitchPower(device, false);
                await _devicePowerControl.SwitchPower(device, true);
                
                device.StateEnum = DeviceState.ERASING;
                
                _context.Entry(device).State = EntityState.Modified;
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
    }
}
