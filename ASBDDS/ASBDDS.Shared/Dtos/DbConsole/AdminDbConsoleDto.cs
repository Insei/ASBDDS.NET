using System;
using ASBDDS.Shared.Dtos.SerialPortSettings;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Requests;

namespace ASBDDS.Shared.Dtos.DbConsole
{
    public class AdminDbConsoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DbConsoleType Type { get; set; }
        public bool Disabled { get; set; }
        public bool IsListening { get; set; }
        public AdminSerialPortSettingsDto SerialSettings { get; set; }
    }
}