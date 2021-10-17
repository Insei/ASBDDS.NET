using System;
using ASBDDS.Shared.Dtos.SerialPortSettings;
using ASBDDS.Shared.Models.Database.DataDb;

namespace ASBDDS.Shared.Dtos.DbConsole
{
    public class AdminDbConsoleUpdateDto
    {
        public string Name { get; set; }
        public DbConsoleType Type { get; set; }
        public bool Disabled { get; set; }
        public AdminSerialPortSettingsUpdateDto SerialSettings { get; set; }
    }
}