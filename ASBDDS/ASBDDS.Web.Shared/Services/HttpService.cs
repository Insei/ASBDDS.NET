using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using ASBDDS.Web.Shared.Interfaces;
using ASBDDS.Web.Shared.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ASBDDS.Web.Client.Services
{
    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private ApiAuthenticationStateProvider _authenticationStateProvider;
        private IConfiguration _configuration;
        private IAuthenticationService _authenticationService;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IConfiguration configuration,
            AuthenticationStateProvider authenticationStateProvider,
            IAuthenticationService authenticationService
        ) {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _configuration = configuration;
            _authenticationStateProvider = (ApiAuthenticationStateProvider)authenticationStateProvider;
            _authenticationService = authenticationService;
        }

        public async Task<T> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await SendRequest<T>(request);

        }
        public async Task<T> Put<T>(string uri, object value = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await SendRequest<T>(request);
        }
        
        public async Task<T> Delete<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            if(value != null)
                request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await SendRequest<T>(request);
        }

        // helper methods
        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var tokenResponse = await _localStorageService.GetItemAsync<TokenResponse>("TokenResponse");
            var tokenExpired = await _authenticationStateProvider.TokenIsExpired();
            var currentProject = await _localStorageService.GetItemAsync<ProjectUserResponse>("CurrentProject");
            if (currentProject != null)
            {
                request.Headers.Add("ProjectId", currentProject.Id.ToString());
            }
            if (!tokenExpired)
                 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            else
            {
                var isRefreshOk = await _authenticationService.RefreshToken();
                if (isRefreshOk)
                {
                    return await SendRequest<T>(request);
                }
                await _authenticationService.Logout();
                return default;
            }

            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _authenticationService.Logout();
                return default;
            }

            var respStr = await response.Content.ReadAsStringAsync();
            // throw exception on error response
            //if (response.IsSuccessStatusCode)
            //{
            //    var resp = JsonConvert.DeserializeObject<ApiResponse>(respStr);
            //    if (resp.Status.Code != 0)
            //        throw new Exception(resp.Status.Message);
            //}

            return JsonConvert.DeserializeObject<T>(respStr);
        }
    }
}