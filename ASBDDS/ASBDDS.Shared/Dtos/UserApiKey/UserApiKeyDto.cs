using System;

namespace ASBDDS.Shared.Dtos.UserApiKey
{
    public class UserApiKeyDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Creation date and time
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Description for API key
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Api Key
        /// </summary>
        public Guid Key { get; set; }
    }
}