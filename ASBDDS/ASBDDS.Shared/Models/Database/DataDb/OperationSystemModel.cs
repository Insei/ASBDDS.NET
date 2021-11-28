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