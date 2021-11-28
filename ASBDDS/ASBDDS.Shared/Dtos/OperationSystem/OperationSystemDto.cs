using System;
using ASBDDS.Shared.Models.Database.DataDb;

namespace ASBDDS.Shared.Dtos.OperationSystem
{
    public class OperationSystemDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// OS Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// OS Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// OS Arch
        /// </summary>
        public string Arch { get; set; }
        /// <summary>
        /// Installation OS boot protocol
        /// </summary>
        public DeviceBootProtocol InstallationProtocol { get; set; }
        /// <summary>
        /// OS boot protocol
        /// </summary>
        public DeviceBootProtocol Protocol { get; set; }
        /// <summary>
        /// Installation boot file
        /// </summary>
        public string InstallationBootFile { get; set; }
        /// <summary>
        /// Options in JSON format
        /// Objects array { "Name": "OpName", "Value": "OpValue" }
        /// </summary>
        public string Options { get; set; }

        public string FullName => Name + "-" + Version + "-" + Arch;
    }
}