using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using ASBDDS.Shared.Models.Requests;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public Guid? EditorId { get; set; }
        public Guid? CreatorId { get; set; }
        public bool Disabled { get; set; }
        public string RefreshToken { get; set; }

        public ApplicationUser() {}
        public ApplicationUser(UserAdminPostRequest user)
        {
            Name = user.Name;
            LastName = user.LastName;
            Created = DateTime.UtcNow;
            Disabled = false;
            UserName = user.UserName;
            Email = user.Email;
        }
    }
}
