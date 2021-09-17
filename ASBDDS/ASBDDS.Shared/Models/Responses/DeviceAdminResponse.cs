using ASBDDS.Shared.Models.Database.DataDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Responses
{
    public class DeviceAdminResponse
    {
        /// <summary>
        /// Device ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Device model
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Device manufactorer
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
        public Guid SwitchPortId { get; set; }
        /// <summary>
        /// Device state
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Power control type
        /// </summary>
        public DevicePowerControlType PowerControlType { get; set; }
        public DeviceAdminResponse(Device _device)
        {
            Id = _device.Id;
            Name = _device.Name;
            Model = _device.Model;
            Manufacturer = _device.Manufacturer;
            Serial = _device.Serial;
            MacAddress = _device.MacAddress;
            SwitchPortId = _device.SwitchPort.Id;
            State = _device.StateEnum.ToString().ToLower();
            PowerControlType = _device.PowerControlType;
        }

        public DeviceAdminResponse() { }
    }
}
