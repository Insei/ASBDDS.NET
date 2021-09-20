using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace ASBDDS.API.Models
{
    public class SeedDatabase
    {
        public static async void Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var dataContext = serviceProvider.GetRequiredService<DataDbContext>();
            dataContext.Database.Migrate();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            // if (!roleManager.Roles.Any(r => r.Name == "Admin"))
            // {
                var adminRoleExist = true;
                var adminRole = await roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    adminRole = new ApplicationRole(){ Name = "Admin", NormalizedName = "ADMIN" };
                    //create the roles and seed them to the database
                    var result = await roleManager.CreateAsync(adminRole);
                    if (!result.Succeeded)
                        adminRoleExist = false;
                }

                var adminUserExist = true;
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var adminUser = await userManager.FindByNameAsync("Admin");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser()
                    {
                        UserName = "Admin", Email = "admin@asbdds.com", Name = "Admin", LastName = "Default"
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (!result.Succeeded)
                        adminUserExist = false;
                }

                if (adminUserExist && adminRoleExist)
                {
                    if(!await userManager.IsInRoleAsync(adminUser, "Admin"))
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                
                if (adminUserExist)
                {
                    var adminDbUser = dataContext.Users.FirstOrDefault(u => u.UserName.Equals("Admin"));
                    if (!dataContext.Projects.Any(p => p.Creator == adminDbUser))
                    {
                        dataContext.Projects.Add(new Project()
                        {
                            Creator = adminDbUser,
                            Name = adminUser.Name + " " + adminUser.LastName + "'s Project"
                        });
                        await dataContext.SaveChangesAsync();
                    }
                }
        }
    }
}
