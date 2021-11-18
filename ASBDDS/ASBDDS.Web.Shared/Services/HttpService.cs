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
        private ILocalStorageService _localStorageService;
        private ApiAuthenticationStateProvider _authenticationStateProvider;
        private IAuthenticationService _authenticationService;

        public HttpService(
            HttpClient httpClient,
            ILocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider,
            IAuthenticationService authenticationService
        ) {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider = (ApiAuthenticationStateProvider)authenticationStateProvider;
            _authenticationService = authenticationService;
        }

        public async Task<ApiResponse<T>> Get<T>(string uri, 
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var apiResp = await SendRequest<ApiResponse<T>>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }
        
        public async Task<ApiResponse> Get(string uri, 
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var apiResp = await SendRequest<ApiResponse>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }

        public async Task<ApiResponse<T>> Post<T>(string uri, object value, 
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            var apiResp = await SendRequest<ApiResponse<T>>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }
        
        public async Task<ApiResponse> Post(string uri, object value, 
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            var apiResp = await SendRequest<ApiResponse>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }

        public async Task<ApiResponse<T>> Put<T>(string uri, object value, 
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            var apiResp = await SendRequest<ApiResponse<T>>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }
        
        public async Task<ApiResponse> Put(string uri, object value, 
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            var apiResp = await SendRequest<ApiResponse>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }

        public async Task<ApiResponse<T>> Delete<T>(string uri, 
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            var apiResp = await SendRequest<ApiResponse<T>>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }
        
        public async Task<ApiResponse> Delete(string uri, 
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            var apiResp = await SendRequest<ApiResponse>(request);
            await RunCallbacks(apiResp, successCallback, failCallback);
            return apiResp;
        }

        // helper methods
        private async Task<int> RunCallbacks(ApiResponse apiResp, Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null)
        {
            if (apiResp.Status.Code == 0 && successCallback != null)
            {
                try
                {
                    await successCallback(apiResp);
                }
                catch (Exception ex)
                {
                    apiResp.Status.Code = 2000;
                    apiResp.Status.Message = ex.Message;
                }
            }
            
            if (apiResp.Status.Code != 0 && failCallback != null)
            {
                await failCallback(apiResp);
            }
            return 0;
        }
        
        private async Task<int> RunCallbacks<T>(ApiResponse<T> apiResp, Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null)
        {
            if (apiResp.Status.Code == 0 && successCallback != null)
            {
                try
                {
                    await successCallback(apiResp);
                }
                catch (Exception ex)
                {
                    apiResp.Status.Code = 2000;
                    apiResp.Status.Message = ex.Message;
                }
            }
            
            if (apiResp.Status.Code != 0 && failCallback != null)
            {
                await failCallback(apiResp);
            }
            return 0;
        }

        private void ProcessApiResponse(ApiResponse apiResp)
        {
            if (apiResp == null || apiResp.Status.Code != 0)
            {
                apiResp ??= new ApiResponse
                {
                    Status =
                    {
                        Message = "Didn't receive a response from the server",
                        Code = 500
                    }
                };
                if(apiResp.Status.Code != 0 && (apiResp.Status.Message == null || apiResp.Status.Message.Trim().Length == 0))
                {
                    apiResp.Status.Message = "Server return error, but not send error message";
                }
            }
        }

        private bool TypeIsApiResponse<T>()
        {
            return typeof(T).GetGenericTypeDefinition() == typeof(ApiResponse<>) ||
                   typeof(T).GetGenericTypeDefinition() == typeof(ApiResponse);
        }
        
        private async Task SetProjectHeader(HttpRequestMessage request)
        {
            var currentProject = await _localStorageService.GetItemAsync<ProjectUserResponse>("CurrentProject");
            if (currentProject != null)
            {
                request.Headers.Add("ProjectId", currentProject.Id.ToString());
            }
        }

        private async Task<bool> CheckAuthorizationsAndSetHeader(HttpRequestMessage request)
        {
            var tokenResponse = await _localStorageService.GetItemAsync<TokenResponse>("TokenResponse");
            var tokenExpired = await _authenticationStateProvider.TokenIsExpired();
            if (!tokenExpired)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                return true;
            }
            var isRefreshOk = await _authenticationService.RefreshToken();
            if (isRefreshOk)
            {
                return await CheckAuthorizationsAndSetHeader(request);
            }
            await _authenticationService.Logout();
            return false;
        }
        
        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            await SetProjectHeader(request);
            var authResult = await CheckAuthorizationsAndSetHeader(request);
            if (!authResult){
                if (TypeIsApiResponse<T>())
                {
                    return MakeApiResponse<T>(401, "You session is expired. Please log in.");
                }
                return default;
            }
            
            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (TypeIsApiResponse<T>())
                {
                    return MakeApiResponse<T>((int)response.StatusCode, response.ReasonPhrase);
                }
                return default;
            }
            
            var respStr = await response.Content.ReadAsStringAsync();

            var apiResp = JsonConvert.DeserializeObject<T>(respStr);
            if (apiResp is ApiResponse)
                ProcessApiResponse(apiResp as ApiResponse);
            
            if (apiResp == null && TypeIsApiResponse<T>())
            {
                apiResp = MakeApiResponse<T>(501, "Can't parse json from server");
            }
            
            return apiResp;
        }

        private TApiResponse MakeApiResponse<TApiResponse>(int code, string message)
        {
            Type genericType = null;
            var genericSourceTypes = typeof(TApiResponse).GetGenericArguments();

            if (genericSourceTypes.Length > 0)
                genericType = typeof(ApiResponse<>).MakeGenericType(genericSourceTypes[0]);
            else
                genericType = typeof(ApiResponse);

            var apiRespT = (TApiResponse) Activator.CreateInstance(genericType);

            var apiResp = apiRespT as ApiResponse;
            
            apiResp.Status.Code = code;
            apiResp.Status.Message = message;

            return apiRespT;
        }
    }
}