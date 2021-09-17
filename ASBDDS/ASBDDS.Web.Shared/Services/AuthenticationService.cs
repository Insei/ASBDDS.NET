using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using ASBDDS.Web.Shared.Interfaces;
using ASBDDS.Web.Shared.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace ASBDDS.Web.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private HttpClient _httpClient;
        private ILocalStorageService _localStorageService;
        private AuthenticationStateProvider _authenticationStateProvider;

        public TokenResponse TokenResponse { get; private set; }

        public AuthenticationService(
            HttpClient httpClient,
            ILocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider
        ) {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task Initialize()
        {
            TokenResponse = await _localStorageService.GetItemAsync<TokenResponse>("TokenResponse");
        }

        private async Task ResetTokenDataAsync()
        {
            await _localStorageService.RemoveItemAsync("TokenResponse");
            TokenResponse = null;
        }

        public async Task<bool> RefreshToken()
        {
            var success = false;
            var expiredToken = await _localStorageService.GetItemAsync<TokenResponse>("TokenResponse");


            if (expiredToken?.RefreshToken == null || expiredToken.AccessToken == null)
            {
                await ResetTokenDataAsync();
                (_authenticationStateProvider as ApiAuthenticationStateProvider)?.Notify();
                return success;
            }
            
            var tokenRefreshRequest = new TokenRefreshRequest
            {
                AccessToken = expiredToken.AccessToken,
                RefreshToken = expiredToken.RefreshToken
            };

            var json = JsonConvert.SerializeObject(tokenRefreshRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/users/jwt/refresh", data);
            var content = await response.Content.ReadAsStringAsync();
            var tokenRefreshResponse = JsonConvert.DeserializeObject<ApiResponse<TokenResponse>>(content);

            if (tokenRefreshResponse != null && tokenRefreshResponse.Status.Code == 0)
            {
                await _localStorageService.RemoveItemAsync("TokenResponse");
                TokenResponse = tokenRefreshResponse.Data;
                await _localStorageService.SetItemAsync("TokenResponse", TokenResponse);
                (_authenticationStateProvider as ApiAuthenticationStateProvider)?.Notify();
                success = true;
            } 
            else
            {
                await _localStorageService.SetItemAsync<TokenResponse>("TokenResponse", null);
                (_authenticationStateProvider as ApiAuthenticationStateProvider)?.Notify();
            }

            return success;
        }

        public async Task Login(string username, string password)
        {
            var tokenRequest = new TokenRequest() {UserName = username, Password = password };

            var json = JsonConvert.SerializeObject(tokenRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/users/jwt", data);
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<ApiResponse<TokenResponse>>(content);

            if (tokenResponse != null && tokenResponse.Status.Code == 0)
            {
                TokenResponse = tokenResponse.Data;
            }
            await _localStorageService.SetItemAsync("TokenResponse", TokenResponse);
            (_authenticationStateProvider as ApiAuthenticationStateProvider)?.Notify();
        }

        public async Task Logout()
        {
            await ResetTokenDataAsync();
            (_authenticationStateProvider as ApiAuthenticationStateProvider)?.Notify();
        }
    }
}