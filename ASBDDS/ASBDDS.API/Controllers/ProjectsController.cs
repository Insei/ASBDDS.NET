using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ASBDDS.API.Controllers
{
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(DataDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all projects as admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/admin/projects/")]
        public async Task<ActionResult<ApiResponse<List<ProjectAdminResponse>>>> GetProjects()
        {
            var resp = new ApiResponse<List<ProjectAdminResponse>>();
            try
            {
                var projects = await _context.Projects.Include(p => p.ProjectDeviceLimits).ToListAsync();
                var _projects = new List<ProjectAdminResponse>();
                foreach (var project in projects)
                {
                    _projects.Add(new ProjectAdminResponse(project));
                }
                resp.Data = _projects;
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        /// <summary>
        /// Get project by ID as admin
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns></returns>
        [HttpGet("api/admin/projects/{id}")]
        public async Task<ActionResult<ApiResponse<ProjectAdminResponse>>> GetProject(Guid id)
        {
            var resp = new ApiResponse<ProjectAdminResponse>();
            try
            {
                var project = await _context.Projects.Where(p => p.Id == id).Include(p => p.ProjectDeviceLimits).FirstOrDefaultAsync();

                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                resp.Data = new ProjectAdminResponse(project);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        /// <summary>
        /// Update project by ID as admin
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="projectReq">See schema</param>
        /// <returns></returns>
        [HttpPut("api/admin/projects/{id}")]
        public async Task<ActionResult<ApiResponse<ProjectAdminResponse>>> PutProject(Guid id, ProjectAdminPutRequest projectReq)
        {
            var resp = new ApiResponse<ProjectAdminResponse>();
            try
            {
                var project = await _context.Projects.Where(p => p.Id == id).Include(p => p.ProjectDeviceLimits).FirstOrDefaultAsync();
                var deviceLimits = project.ProjectDeviceLimits;

                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                project.Name = projectReq.Name;
                project.DefaultVlan = projectReq.DefaultVlan;
                project.AllowCustomBootloaders = projectReq.AllowCustomBootloaders;
                project.Disabled = projectReq.Disabled;

                project.ProjectDeviceLimits.Clear();
                foreach(var deviceLimit in projectReq.ProjectDeviceLimits)
                {
                    project.ProjectDeviceLimits.Add(new ProjectDeviceLimit(deviceLimit, project));
                }

                _context.Entry(project).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new ProjectAdminResponse(project);
                
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        /// <summary>
        /// Add new project as admin
        /// </summary>
        /// <param name="projectReq">See schema</param>
        /// <returns></returns>
        [HttpPost("api/admin/projects/")]
        public async Task<ApiResponse<ProjectAdminResponse>> PostProject(ProjectAdminPostRequest projectReq)
        {
            var resp = new ApiResponse<ProjectAdminResponse>();
            try
            {
                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var project = new Project()
                {
                    Name = projectReq.Name,
                    DefaultVlan = projectReq.DefaultVlan,
                    AllowCustomBootloaders = projectReq.AllowCustomBootloaders,
                    ProjectDeviceLimits = new List<ProjectDeviceLimit>(),
                    Creator = user
                };

                foreach(var deviceLimit in projectReq.ProjectDeviceLimits)
                {
                    project.ProjectDeviceLimits.Add(new ProjectDeviceLimit(deviceLimit, project));
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                resp.Data = new ProjectAdminResponse(project);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        /// <summary>
        /// Delete project by ID as admin
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns></returns>
        [HttpDelete("api/admin/projects/{id}")]
        public async Task<ActionResult<ApiResponse<ProjectAdminResponse>>> DeleteProject(Guid id)
        {
            var resp = new ApiResponse<ProjectAdminResponse>();
            try
            {
                var project = await _context.Projects.Where(d => d.Id == id).Include(d => d.ProjectDeviceLimits).FirstOrDefaultAsync();
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                // foreach (var deviceLimit in project.ProjectDeviceLimits)
                // {
                //     _context.ProjectDeviceLimits.Remove(deviceLimit);
                // }
                project.Disabled = true;
                _context.Entry(project).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new ProjectAdminResponse(project);
                
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        /// <summary>
        /// Get all projects as user
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/projects/")]
        public async Task<ActionResult<ApiResponse<List<ProjectUserResponse>>>> GetUserProjects()
        {
            var resp = new ApiResponse<List<ProjectUserResponse>>();
            try
            {
                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var activeProjects = await _context.Projects.Where(p => !p.Disabled && p.Creator == user).ToListAsync();
                var _projects = new List<ProjectUserResponse>();
                foreach (var project in activeProjects)
                {
                    _projects.Add(new ProjectUserResponse(project));
                }
                resp.Data = _projects;
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        /// <summary>
        /// Get project by ID as user
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns></returns>
        [HttpGet("api/projects/{id}")]
        public async Task<ActionResult<ApiResponse<ProjectUserResponse>>> GetUserProject(Guid id)
        {
            var resp = new ApiResponse<ProjectUserResponse>();
            try
            {
                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var project = await _context.Projects.Where(p => p.Id == id && !p.Disabled && p.Creator == user)
                    .Include(p => p.ProjectDeviceLimits).FirstOrDefaultAsync();

                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                resp.Data = new ProjectUserResponse(project);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        /// <summary>
        /// Update project by ID as user
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="projectReq">See schema</param>
        /// <returns></returns>
        [HttpPut("api/projects/{id}")]
        public async Task<ActionResult<ApiResponse<ProjectUserResponse>>> PutUserProject(Guid id, ProjectUserPutRequest projectReq)
        {
            var resp = new ApiResponse<ProjectUserResponse>();
            try
            {
                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var project = await _context.Projects.Where(p => !p.Disabled && p.Id == id && p.Creator == user).FirstOrDefaultAsync();
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                project.Name = projectReq.Name;

                _context.Entry(project).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new ProjectUserResponse(project);

            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        /// <summary>
        /// Add new project as user
        /// </summary>
        /// <param name="projectReq"></param>
        /// <returns></returns>
        [HttpPost("api/projects/")]
        public async Task<ApiResponse<ProjectUserResponse>> PostUserProject(ProjectUserPostRequest projectReq)
        {
            var resp = new ApiResponse<ProjectUserResponse>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName.Equals(HttpContext.User.Identity.Name));
                var project = new Project()
                {
                    Name = projectReq.Name,
                    Creator = user
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                resp.Data = new ProjectUserResponse(project);
            }
            catch (Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }

            return resp;
        }

        /// <summary>
        /// Delete project by ID as user
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns></returns>
        [HttpDelete("api/projects/{id}")]
        public async Task<ActionResult<ApiResponse<ProjectUserResponse>>> DeleteUserProject(Guid id)
        {
            var resp = new ApiResponse<ProjectUserResponse>();
            try
            {
                //TODO: We can't delete project if in this project exist active device rents.
                var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var project = await _context.Projects.Where(p => p.Id == id && !p.Disabled && p.Creator == user)
                    .Include(p => p.ProjectDeviceLimits).FirstOrDefaultAsync();
                if (project == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "project not found";
                    return resp;
                }

                // foreach (var deviceLimit in project.ProjectDeviceLimits)
                // {
                //     _context.ProjectDeviceLimits.Remove(deviceLimit);
                // }
                project.Disabled = true;
                _context.Entry(project).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new ProjectUserResponse(project);
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
