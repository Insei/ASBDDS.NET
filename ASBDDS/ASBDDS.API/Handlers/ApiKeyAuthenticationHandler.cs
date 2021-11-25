using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASBDDS.NET.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DataDbContext _context;
 
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            DataDbContext context) 
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }
 
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKeyHeader = "X-API-Key";
            if (!Request.Headers.ContainsKey(apiKeyHeader))
                return AuthenticateResult.Fail("Unauthorized");
 
            string authorizationHeader = Request.Headers[apiKeyHeader];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            var apikey = authorizationHeader.Trim();
            if (string.IsNullOrEmpty(apikey))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
 
            try
            {
                return await ValidateApiKey(apikey);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }
 
        private async Task<AuthenticateResult> ValidateApiKey(string token)
        {
            try
            {
                var key = Guid.Parse(token);
                var userApiKeyEntity = await _context.UserApiKeys.Where(t => t.Key == key).Include(a => a.Creator)
                    .FirstOrDefaultAsync();
                if (userApiKeyEntity == null)
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                var identity = await GetClaimsIdentity(userApiKeyEntity.Creator);
                var roles = await GetUserRoles(userApiKeyEntity.Creator);
                var principal = new System.Security.Principal.GenericPrincipal(identity, roles.ToArray());
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        private async Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user)
        {
            var userRoles = await GetUserRoles(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var claimsIdentity =
                new ClaimsIdentity(claims, "APIKey", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            var userRoles = await  _context.UserRoles.Where(u => u.UserId == user.Id).Select(u => u.RoleId).ToListAsync();
            var roles = await _context.Roles.Where(r => userRoles.Contains(r.Id)).Select(r => r.Name).ToListAsync();
            return roles;
        }
    }
}