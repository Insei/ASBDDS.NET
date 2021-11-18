using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Responses
{
    public class ResponseStatus
    {
        /// <summary>
        /// Response code:
        /// 0 -> OK;
        /// 1 and more -> error;
        /// </summary>
        public int Code { get; set; } = 0;
        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; }

    }

    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse(): base(){}

        public T Data { get; set; }
    }

    public class ApiResponse
    {
        public ApiResponse()
        {
            Status = new ResponseStatus();
        }

        public ResponseStatus Status { get; }
    }
}
