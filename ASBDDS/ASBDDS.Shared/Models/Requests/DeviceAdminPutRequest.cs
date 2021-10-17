using ASBDDS.Shared.Models.Database.DataDb;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ASBDDS.Shared.Models.Requests
{
    public class DeviceAdminPutRequest
    {
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Device model
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Device manufacturer
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// Device serial
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// Device mac address
        /// </summary>
        public string MacAddress { get; set; }
        /// <summary>
        /// Switch port ID
        /// </summary>
        public Guid? SwitchPortId { get; set; }
        /// <summary>
        /// Power control type
        /// </summary>
        public DevicePowerControlType PowerControlType { get; set; }
        public Guid? ConsoleId { get; set; }
        public DeviceAdminPutRequest() { }
    }
}
