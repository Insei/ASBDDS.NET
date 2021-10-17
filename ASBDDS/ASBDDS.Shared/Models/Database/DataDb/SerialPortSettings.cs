using ASBDDS.Shared.Models.Requests;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public enum DbSerialPortParity
    {
        None,
        Odd,
        Even,
        Mark,
        Space
    }

    public enum DbSerialPortHandshake
    {
        None = 0,
        XOnXOff,
        RequestToSend,
        RequestToSendXOnXOff
    }
    
    public enum DbSerialPortStopBits
    {
        None = 0, 
        One,
        Two,
        OnePointFive,
    }

    public class SerialPortSettings : DbBaseModel
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public DbSerialPortStopBits StopBits { get; set; }
        public DbSerialPortParity Parity { get; set; }
        public DbSerialPortHandshake Handshake { get; set; }
    }
}