using System;
using System.Threading.Tasks;
using ASBDDS.Shared.Models.Responses;

namespace ASBDDS.Web.Shared.Interfaces
{
    public interface IHttpService
    {
        // Task<T> Get<T>(string uri);
        // Task<T> Post<T>(string uri, object value);
        // Task<T> Delete<T>(string uri);
        // Task<T> Put<T>(string uri, object value);

        Task<ApiResponse<T>> Get<T>(string uri,
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null);
        Task<ApiResponse> Get(string uri,
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null);
        Task<ApiResponse<T>> Post<T>(string uri, object value, 
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null);
        Task<ApiResponse> Post(string uri, object value, 
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null);
        Task<ApiResponse<T>> Put<T>(string uri, object value,
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null);
        Task<ApiResponse> Put(string uri, object value,
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null);
        Task<ApiResponse<T>> Delete<T>(string uri,
            Func<ApiResponse<T>, Task> successCallback = null,
            Func<ApiResponse<T>, Task> failCallback = null);
        Task<ApiResponse> Delete(string uri,
            Func<ApiResponse, Task> successCallback = null,
            Func<ApiResponse, Task> failCallback = null);
    }
}