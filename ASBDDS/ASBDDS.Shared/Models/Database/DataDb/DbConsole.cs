using System;
using ASBDDS.Shared.Models.Requests;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public enum DbConsoleType
    {
        Unknown = 0,
        Serial = 1
    }
    
    public class DbConsole : DbBaseModel
    {
        public string Name { get; set; }
        public DbConsoleType Type { get; set; }
        public bool Disabled { get; set; }
        public virtual SerialPortSettings SerialSettings { get; set; }
    }
}