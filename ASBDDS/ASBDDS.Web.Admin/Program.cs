using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ASBDDS.Web.Client.Services;
using ASBDDS.Web.Shared.Interfaces;
using ASBDDS.Web.Shared.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ASBDDS.Web.Admin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped(
                _ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress.Replace("admin/", "")) });
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            var host = builder.Build();
            
            var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
            await authenticationService.Initialize();

            await host.RunAsync();
        }
    }
}
