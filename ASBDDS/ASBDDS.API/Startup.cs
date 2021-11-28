using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.API.Models;
using ASBDDS.API.Servers.TFTP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using ASBDDS.API.Servers.DHCP;
using ASBDDS.NET.Handlers;
using ASBDDS.NET.Interfaces;
using ASBDDS.NET.MappingProfiles;
using ASBDDS.NET.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DHCPServer = ASBDDS.API.Servers.DHCP.DHCPServer;

namespace ASBDDS.NET
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthOptions = Configuration.GetSection("API:Auth").Get<AuthOptions>();

            var dnetIpStr = Configuration.GetValue<string>("Networks:Devices:IP");

            DhcpServer = DHCPServerHelper.Create(configuration);
            TftpServer = new TFTPServer(dnetIpStr, 69, DhcpServer);
            ConsolesManager = new ConsolesManager();
        }

        public IConfiguration Configuration { get; }
        private TFTPServer TftpServer { get; }
        private DHCPServer DhcpServer { get; }
        private AuthOptions AuthOptions { get; }
        private ConsolesManager ConsolesManager { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DataDbConnection"),
                x => x.MigrationsAssembly("ASBDDS.API")), ServiceLifetime.Transient);
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<DataDbContext>();
            services.AddSingleton(_ => TftpServer);
            services.AddSingleton(_ => DhcpServer);
            services.AddSingleton(_ => AuthOptions);
            services.AddSingleton(_ => ConsolesManager);
            services.AddScoped<DevicePowerControlManager>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = "smart";
                    option.DefaultChallengeScheme = "smart";
                })
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("APIKey", null)
                .AddPolicyScheme("smart", "Authorization Bearer JWT or user API key", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        return authHeader?.StartsWith("Bearer ") == true ? JwtBearerDefaults.AuthenticationScheme : "APIKey";
                    };
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
 
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        ValidateLifetime = true,

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });
            
            services.AddControllersWithViews().AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddRazorPages();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "ASBDDS.NET",
                        Version = "v1"});
                var filePathShared = Path.Combine(System.AppContext.BaseDirectory, "ASBDDS.Shared.xml");
                var filePathApi = Path.Combine(System.AppContext.BaseDirectory, "ASBDDS.API.xml");
                c.IncludeXmlComments(filePathShared);
                c.IncludeXmlComments(filePathApi);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.ApiKey,  
                    Scheme = "Bearer",  
                    BearerFormat = "JWT",  
                    In = ParameterLocation.Header,  
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",  
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                        new OpenApiSecurityScheme  
                        {  
                            Reference = new OpenApiReference  
                            {  
                                Type = ReferenceType.SecurityScheme,  
                                Id = "Bearer"  
                            }  
                        },  
                        new string[] {}  
  
                    }  
                });
                c.AddSecurityDefinition("X-API-Key", new OpenApiSecurityScheme()  
                {  
                    Name = "X-API-Key",  
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "APIKey Authorization header using the X-API-Key scheme. \r\n\r\n Enter your API key in the text input below.\r\n\r\nExample: \" 12345abcdef\"",  
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                        new OpenApiSecurityScheme  
                        {  
                            Reference = new OpenApiReference  
                            {  
                                Type = ReferenceType.SecurityScheme,  
                                Id = "X-API-Key"  
                            }  
                        },  
                        new string[] {}  
  
                    }  
                });
            });
            services.AddAutoMapper(
                typeof(DbConsoleMappings), 
                typeof(SerialPortSettingsMappings), 
                typeof(ConsoleOutputMappings),
                typeof(UserApiKeyMappings));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Apply unapplied migrations when the service starts, or create a database if it doesn't exist. 
            //If no users are found, default users from config will be created.
            var serviceProvider = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider;
            SeedDatabase.Initialize(serviceProvider, Configuration);

            // Configure and start DHCP Server
            DHCPServerHelper.Initialize(serviceProvider);
            // Configure and start TFTP Server
            TFTPServerHelper.Initialize(serviceProvider);
            // Configure consoles from DB and start listening.
            ConsolesManagerHelper.Initialize(serviceProvider);
            //app.UseHttpsRedirection();

            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/admin"), adminPanel =>
            {
                adminPanel.UseBlazorFrameworkFiles("/admin");
                adminPanel.UseStaticFiles("/admin");
                adminPanel.UseRouting();
                adminPanel.UseAuthentication();
                adminPanel.UseAuthorization();
                adminPanel.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapFallbackToFile("/admin/{*path:nonfile}",
                        "admin/index.html");
                });
            });

            app.MapWhen(ctx => !ctx.Request.Path.StartsWithSegments("/admin"), userPanel =>
            {
                userPanel.UseBlazorFrameworkFiles();
                userPanel.UseStaticFiles();
                userPanel.UseSwagger();
                userPanel.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "ASBDDS.NET v1"));
                userPanel.UseRouting();
                userPanel.UseAuthentication();
#pragma warning disable ASP0001 // Authorization middleware is incorrectly configured.
                userPanel.UseAuthorization();
#pragma warning restore ASP0001 // Authorization middleware is incorrectly configured.
                userPanel.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapFallbackToFile("/{*path:nonfile}",
                        "index.html");
                });
            });
        }
    }
}
