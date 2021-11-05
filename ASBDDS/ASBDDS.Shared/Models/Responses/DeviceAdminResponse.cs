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
        /// Device power state
        /// </summary>
        public DevicePowerState PowerState { get; set; }
        /// <summary>
        /// Device machine state
        /// </summary>
        public DeviceMachineState MachineState { get; set; }
        /// <summary>
        /// Power control type
        /// </summary>
        public DevicePowerControlType PowerControlType { get; set; }

        public Guid? ConsoleId { get; set; }

        public DeviceAdminResponse(Device device)
        {
            Id = device.Id;
            Name = device.Name;
            Model = device.Model;
            Manufacturer = device.Manufacturer;
            Serial = device.Serial;
            MacAddress = device.MacAddress;
            SwitchPortId = device.SwitchPort.Id;
            PowerState = device.PowerState;
            MachineState = device.MachineState;
            PowerControlType = device.PowerControlType;
            ConsoleId = device.Console?.Id;
        }

        public DeviceAdminResponse() { }
    }
}
