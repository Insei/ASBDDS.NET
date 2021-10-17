using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class Device : DbBaseModelWithoutId
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DeviceState StateEnum { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public string MacAddress { get; set; }
        public virtual SwitchPort SwitchPort { get; set; }
        public DevicePowerControlType PowerControlType { get; set; }
        public virtual DbConsole Console { get; set; }
    }
}
