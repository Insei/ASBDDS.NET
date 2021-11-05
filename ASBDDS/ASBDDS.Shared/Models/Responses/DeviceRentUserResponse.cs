using ASBDDS.Shared.Models.Database.DataDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Responses
{
    public class DeviceRentUserResponse
    {
        /// <summary>
        /// Device object
        /// </summary>
        private Device device { get; set; }
        /// <summary>
        /// Rent ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The ID of the user who created rent
        /// </summary>
        public Guid CreatorId { get; set; }
        /// <summary>
        /// The project ID to which rent is attached
        /// </summary>
        public Guid ProjectId { get; set; }
        /// <summary>
        /// Date of creation rent
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// End date of rent
        /// </summary>
        public DateTime? Closed { get; set; }
        /// <summary>
        /// Serial number of the leased device
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// Mac address number of the leased device
        /// </summary>
        public string MacAddress { get; set; }
        /// <summary>
        /// Link to IPXE network bootloader
        /// </summary>
        public string IPXEUrl { get; set; }
        /// <summary>
        /// Device power state
        /// </summary>
        public DevicePowerState PowerState { get; set; }
        /// <summary>
        /// Device machine state
        /// </summary>
        public DeviceMachineState MachineState { get; set; }
        public string Status { get; set; }

        public DeviceRentUserResponse(DeviceRent deviceRent)
        {
            device = deviceRent.Device;
            Id = deviceRent.Id;
            Name = deviceRent.Name;
            if (deviceRent.Creator != null)
                CreatorId = deviceRent.Creator.Id;
            if (deviceRent.Project != null)
                ProjectId = deviceRent.Project.Id;
            Created = deviceRent.Created;
            Closed = deviceRent.Closed;
            IPXEUrl = deviceRent.IpxeUrl;
            Serial = deviceRent.Device.Serial;
            MacAddress = deviceRent.Device.MacAddress;
            PowerState = device.PowerState;
            MachineState = device.MachineState;
            Status = deviceRent.Status.ToString().ToLower();
        }

        public DeviceRentUserResponse() { }
    }
}
