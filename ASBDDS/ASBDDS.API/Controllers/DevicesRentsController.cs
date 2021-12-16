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
using ASBDDS.API.Models.Utils;
using ASBDDS.Shared.Dtos;
using AutoMapper;

namespace ASBDDS.API.Controllers
{
    
    [ApiController]
    [Authorize]
    [Route("api/devices/rents")]
    public class DevicesRentsController : ControllerBase
    {
        private const string ProjectIdHeader = "X-Project-Id";
        private readonly DataDbContext _context;
        private readonly DevicePowerControlManager _devicePowerControl;
        private readonly ConsolesManager _consolesManager;
        private readonly IMapper _mapper;

        public DevicesRentsController(DataDbContext context, 
            DevicePowerControlManager devicePowerControl, 
            ConsolesManager consolesManager,
            IMapper mapper)
        {
            _context = context;
            _devicePowerControl = devicePowerControl;
            _consolesManager = consolesManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all devices rents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<DeviceRentUserResponse>>> GetUserDevicesRents([FromHeader(Name=ProjectIdHeader)][Required] Guid projectId)
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
                    .Select(rent => new DeviceRentUserResponse(rent)).ToListAsync();
                resp.Data = devicesRents;
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
        public async Task<ApiResponse<DeviceRentUserResponse>> GetUserDeviceRent([FromHeader(Name=ProjectIdHeader)][Required] Guid projectId, Guid id)
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
                var deviceRent = await _context.DeviceRents.Where(dr => dr.Project == project && dr.Closed == null && dr.Id == id)
                    .Include(dr => dr.Device).FirstOrDefaultAsync();
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
                                                                                        [FromHeader(Name=ProjectIdHeader)][Required] Guid projectId)
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
        public async Task<ApiResponse<DeviceRentUserResponse>> CreateUserDeviceRent([FromHeader(Name=ProjectIdHeader)][Required] Guid projectId, 
                                                                                                        DeviceRentUserPostRequest devRentReq)
        {
            var resp = new ApiResponse<DeviceRentUserResponse>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                var project = await _context.Projects.Include(p => p.ProjectDeviceLimits).FirstOrDefaultAsync(p => p.Id == projectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }
                
                var limit = project.ProjectDeviceLimits.FirstOrDefault(l =>
                    l.Model == devRentReq.Model && l.Manufacturer == devRentReq.Manufacturer);
                if (limit == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "In this project we can't rent this device model.";
                    return resp;
                }

                var countInRentByProjectAndWithSelectedModel = _context.DeviceRents.Include(dr => dr.Device)
                    .Count(dr => dr.Status == DeviceRentStatus.ACTIVE
                                 && dr.Device.Manufacturer == devRentReq.Manufacturer
                                 && dr.Device.Model == devRentReq.Model);
                if (limit.Count <= countInRentByProjectAndWithSelectedModel)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "You cannot rent a device of this model due to project restrictions.";
                    return resp;
                }
                
                var devicesIdInRents = await _context.DeviceRents.Where(r => r.Closed == null).Select(r => r.Device.Id).ToListAsync();
                var deviceModel = devRentReq.Model.Trim().Replace("_", " ");
                var deviceManufacturer = devRentReq.Manufacturer.Trim().Replace("_", " ");
                var freeDevice = await _context.Devices
                    .FirstOrDefaultAsync(d => !devicesIdInRents.Contains(d.Id)
                                              && d.Manufacturer.Equals(deviceManufacturer, StringComparison.OrdinalIgnoreCase)
                                              && d.Model.Equals(deviceModel, StringComparison.OrdinalIgnoreCase));
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
                // We setup Creating state, after ipxe.cfg url is called this state change to provisioning
                deviceRent.Device.MachineState = DeviceMachineState.Creating;

#pragma warning disable 4014
                _devicePowerControl.SwitchPower(deviceRent.Device, DevicePowerAction.PowerOn);
#pragma warning restore 4014
                
                deviceRent.Device.PowerState = DevicePowerState.PowerOn;
                _context.Entry(deviceRent.Device).State = EntityState.Modified;
                
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
        /// <param name="id">Device rent ID</param>
        /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<DeviceRentUserResponse>> CloseUserDeviceRent([FromHeader(Name=ProjectIdHeader)][Required] Guid projectId, 
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
#pragma warning disable 4014
                _devicePowerControl.SwitchPower(deviceRent.Device, DevicePowerAction.Reboot);
#pragma warning restore 4014

                deviceRent.Device.MachineState = DeviceMachineState.Erasing;
                
                _context.Entry(deviceRent.Device).State = EntityState.Modified;
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
        /// Power off device by rent ID
        /// </summary>
        /// <param name="id">Device rent ID</param>
        /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [HttpPost("{id}/poweroff")]
        public async Task<ApiResponse<DeviceRentUserResponse>> PowerOffDeviceByRent(
            [FromHeader(Name=ProjectIdHeader)] [Required] Guid projectId,
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

                if ((deviceRent.Device.MachineState is DeviceMachineState.Erasing or DeviceMachineState.Creating or DeviceMachineState.Provisioning) 
                    || deviceRent.Device.PowerState == DevicePowerState.PowerOff)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "You cannot turn off the device in this state";
                    return resp;
                }
                var device = deviceRent.Device;
#pragma warning disable 4014
                _devicePowerControl.SwitchPower(device, DevicePowerAction.PowerOff);
#pragma warning restore 4014
                device.PowerState = DevicePowerState.PowerOff;
                
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Power on device by rent ID
        /// </summary>
        /// <param name="id">Device rent ID</param>
        /// <param name="projectId">Project id</param>
        /// <returns></returns>
        [HttpPost("{id}/poweron")]
        public async Task<ApiResponse<DeviceRentUserResponse>> PowerOnDeviceByRent(
            [FromHeader(Name=ProjectIdHeader)] [Required] Guid projectId,
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

                if (deviceRent.Device.PowerState != DevicePowerState.PowerOff)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "You cannot turn on the device in this state";
                    return resp;
                }
                
                var device = deviceRent.Device;
#pragma warning disable 4014
                _devicePowerControl.SwitchPower(device, DevicePowerAction.PowerOn);
#pragma warning restore 4014
                device.PowerState = DevicePowerState.PowerOn;
                
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        /// <summary>
        /// Get device console output by rent
        /// </summary>
        /// <param name="id">device rent id</param>
        /// <param name="projectId">project id</param>
        /// <returns></returns>
        [HttpGet("{id}/console/output")]
        public async Task<ApiResponse<List<ConsoleOutputDto>>> GetDeviceConsoleOutputByRent([FromHeader(Name=ProjectIdHeader)][Required] Guid projectId, Guid id)
        {
            var resp = new ApiResponse<List<ConsoleOutputDto>>();
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Project not found";
                    return resp;
                }
                var deviceRent = await _context.DeviceRents.
                    Where(dr => dr.Project == project && dr.Closed == null && dr.Id == id).
                    Include( dr => dr.Device).ThenInclude(d => d.Console).FirstOrDefaultAsync();
                
                if(deviceRent == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "device rent not found";
                    return resp;
                }

                if (deviceRent.Device.Console == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "the device does not have a console";
                    return resp;
                }
                resp.Data = _consolesManager.GetConsoleOutput(deviceRent.Device.Console, deviceRent.Created)
                    .Select(output => _mapper.Map<ConsoleOutputDto>(output)).TakeLast(500).ToList();
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
