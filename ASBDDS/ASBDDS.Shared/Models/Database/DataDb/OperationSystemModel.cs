using System;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class OperationSystemModel : DbBaseModel
    {
        /// <summary>
        /// Disable flag
        /// </summary>
        public bool Disabled { get; set; }
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
        /// Installation protocol
        /// </summary>
        public DeviceBootProtocol InstallationProtocol { get; set; }
        /// <summary>
        /// Boot file
        /// </summary>
        public string InstallationBootFile { get; set; }
        /// <summary>
        /// Required boot protocol after installation
        /// </summary>
        public DeviceBootProtocol Protocol { get; set; }
        /// <summary>
        /// Flag that the operating system is only needed for internal usage
        /// </summary>
        public bool OnlyInternalUsage { get; set; }
        /// <summary>
        /// Options in JSON format
        /// Objects array { "Name": "OpName", "Value": "OpValue" }
        /// </summary>
        public string Options { get; set; }
        /// <summary>
        /// OS full name in system
        /// </summary>
        public string GetFullName() => Name + "-" + Version + "-" + Arch;
    }
}