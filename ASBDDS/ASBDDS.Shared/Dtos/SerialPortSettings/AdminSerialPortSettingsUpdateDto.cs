using ASBDDS.Shared.Models.Database.DataDb;

namespace ASBDDS.Shared.Dtos.SerialPortSettings
{
    public class AdminSerialPortSettingsUpdateDto
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public DbSerialPortStopBits StopBits { get; set; }
        public DbSerialPortParity Parity { get; set; }
        public DbSerialPortHandshake Handshake { get; set; }
    }
}