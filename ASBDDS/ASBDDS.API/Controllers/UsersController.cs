using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ASBDDS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ASBDDS.API.Controllers 
{
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(DataDbContext context, UserManager<ApplicationUser> userManager, 
                                    SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: api/ApplicationUsers
        /// <summary>
        /// Get all application users
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/admin/users/")]
        public async Task<ApiResponse<List<UserAdminResponse>>> GetUsers()
        {
            var resp = new ApiResponse<List<UserAdminResponse>>();
            try
            {
                var dbUsers = await _context.Users.ToListAsync();
                resp.Data = new List<UserAdminResponse>();
                foreach (var dbUser in  dbUsers)
                {
                    resp.Data.Add(new UserAdminResponse(dbUser));
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // GET: api/ApplicationUsers/5
        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        [HttpGet("api/admin/users/{id}")]
        public async Task<ApiResponse<UserAdminResponse>> GetUser(Guid id)
        {
            var resp = new ApiResponse<UserAdminResponse>();
            try
            {
                // TODO: Only admin can read all users. Default user can read - only his account.
                var dbUser = await _context.Users.FindAsync(id);
                if (dbUser == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "not found";
                    return resp;
                }
                resp.Data = new UserAdminResponse(dbUser);
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // PUT: api/ApplicationUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="reqUser">See schema</param>
        /// <returns></returns>
        [HttpPut("api/admin/users/{id}")]
        public async Task<ApiResponse<UserAdminResponse>> PutUser(Guid id, UserAdminPutRequest reqUser)
        {
            var resp = new ApiResponse<UserAdminResponse>();
            try
            {
                if (id != reqUser.Id)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "bad user id";
                    return resp;
                }
                // TODO: Only admin can read all users. Default user can read - only his account.
                var dbUser = await _context.Users.FindAsync(id);
                if (dbUser == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "not found";
                    return resp;
                }

                updateUserAdmin(dbUser, reqUser);
                _context.Entry(dbUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                resp.Data = new UserAdminResponse(dbUser);
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // POST: api/ApplicationUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="reqUser">See schema</param>
        /// <returns></returns>
        [HttpPost("api/admin/users/")]
        public async  Task<ApiResponse<UserAdminResponse>> PostUser(UserAdminPostRequest reqUser)
        {
            var resp = new ApiResponse<UserAdminResponse>();
            try
            {
                //TODO: Save user that this user
                var user = new ApplicationUser(reqUser)
                {
                    
                };
                var result = await _userManager.CreateAsync(user, reqUser.Password);
                if (result.Succeeded)
                {
                    var userDefaultProject = new Project()
                    {
                        Creator = user,
                        Name = user.Name + " " + user.LastName + "'s Project",
                    };
                    _context.Projects.Add(userDefaultProject);
                    await _context.SaveChangesAsync();
                    resp.Data = new UserAdminResponse(user);
                }
                else
                {
                    resp.Status.Code = 1;
                    foreach (var error in result.Errors)
                        resp.Status.Message += error.Code + " \n";
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        // DELETE: api/ApplicationUsers/5
        /// <summary>
        /// Delete user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        [HttpDelete("api/admin/users/{id}")]
        public async Task<ApiResponse> DisableUser(Guid id)
        {
            var resp = new ApiResponse();
            try
            {
                var dbUser = await _context.Users.FindAsync(id);
                if (dbUser == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "not found";
                    return resp;
                }

                dbUser.Disabled = true;
                _context.Entry(dbUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        private void updateUserAdmin(ApplicationUser dbUser, UserAdminPutRequest reqUser)
        {
            dbUser.Disabled = reqUser.Disabled;
            dbUser.Name = reqUser.Name;
            dbUser.LastName = reqUser.LastName;
            dbUser.Updated = DateTime.UtcNow;
            dbUser.Email = reqUser.Email;
            //TODO: write Editor user.
        }

        /// <summary>
        /// Get new JWT token
        /// </summary>
        /// <param name="request">See schema</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("api/users/jwt")]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> GenerateToken(TokenRequest request)
        {
            var resp = new ApiResponse<TokenResponse>();
            try
            {
                var dbuser = await _userManager.FindByNameAsync(request.UserName);
                if (dbuser == null)
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "user not found";
                    return resp;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(dbuser, request.Password, false);
                if (result.Succeeded)
                {
                    var userClaims = await GetClaimsIdentity(dbuser);
                    var now = DateTime.UtcNow;
                    var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));
                    // создаем JWT-токен
                    var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: userClaims.Claims,
                        expires: expires,
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                    dbuser.RefreshToken = GenerateRefreshToken();
                    _context.Entry(dbuser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    resp.Data = new TokenResponse()
                    {
                        AccessToken = encodedJwt,
                        UserName = userClaims.Name,
                        Expires = jwt.ValidTo,
                        RefreshToken = dbuser.RefreshToken
                    };
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        [AllowAnonymous]
        [HttpPost("api/users/jwt/refresh")]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> Refresh(TokenRefreshRequest request)
        {   
            var resp = new ApiResponse<TokenResponse>();
            try
            {
                var principal = GetPrincipalFromExpiredToken(request.AccessToken);
                var username = principal.Identity.Name;
                if (username != null)
                {
                    var dbuser = await _userManager.FindByNameAsync(username);

                    if (dbuser.RefreshToken != request.RefreshToken)
                    {
                        resp.Status.Code = 1;
                        resp.Status.Message = "refresh token is not valid.";
                    }
                    
                    var userClaims = await GetClaimsIdentity(dbuser);
                    var now = DateTime.UtcNow;
                    var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

                    var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: userClaims.Claims,
                        expires: expires,
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    
                    dbuser.RefreshToken = GenerateRefreshToken();
                    _context.Entry(dbuser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    resp.Data = new TokenResponse()
                    {
                        AccessToken = encodedJwt,
                        UserName = userClaims.Name,
                        Expires = jwt.ValidTo,
                        RefreshToken = dbuser.RefreshToken
                    };
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
            };
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}