using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class DataDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim,
        ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) { }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Switch> Switches { get; set; }
        public DbSet<SwitchPort> SwitchPorts { get; set; }
        public DbSet<Router> Routers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<ProjectDeviceLimit> ProjectDeviceLimits { get; set; }
        public DbSet<DeviceRent> DeviceRents { get; set; }
        public DbSet<UserApiKey> UserApiKeys { get; set; }
        public DbSet<DHCPLeaseDb> DHCPLeases { get; set; }
        public DbSet<DbConsole> Consoles { get; set; }
        public DbSet<SerialPortSettings> SerialPortsSettings { get; set; }
    }
}
