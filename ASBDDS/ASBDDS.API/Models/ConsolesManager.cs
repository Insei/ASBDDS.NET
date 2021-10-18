using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO.Ports;
using System.Linq;
using ASBDDS.Shared.Models.Database.DataDb;
using GitHub.JPMikkers.DHCP;
using Db = ASBDDS.Shared.Models.Database.DataDb;
namespace ASBDDS.API.Models
{
    public class ConsoleOutput
    {
        public DateTime DateUtc { get; set; }
        public string Text { get; set; }
    }
    
    public class ConsoleManagerUnit : IDisposable
    {
        public Guid? ConsoleId => Console?.Id;
        private DbConsole Console { get; set; }
        private SerialPort SerialPort { get; set; }
        public List<ConsoleOutput> Output { get; }

        public ConsoleManagerUnit(DbConsole console)
        {
            Output = new List<ConsoleOutput>();
            Console = console;
            Update(console);
        }
        
        private void SerialDataReceived(object sender,
            SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            Output.Add(new ConsoleOutput() { DateUtc = DateTime.Now.ToUniversalTime(), Text = sp.ReadExisting()});
        }
        
        private void AddSerialPort()
        {
            var settings = Console.SerialSettings;
            SerialPort = new SerialPort();
                    
            SerialPort.BaudRate = settings.BaudRate;
            SerialPort.PortName = settings.PortName;
            SerialPort.Handshake = (Handshake)settings.Handshake;
            SerialPort.Parity = (Parity)settings.Parity;
            SerialPort.DataBits = settings.DataBits;
            SerialPort.StopBits = (StopBits)settings.StopBits;
            SerialPort.DataReceived += SerialDataReceived;
        }

        private void CloseAndDisposeSerialPort()
        {
            if(SerialPort?.IsOpen == true)
                SerialPort?.Close();
            SerialPort?.Dispose();
        }

        public void Update(DbConsole console)
        {
            Console = console;
            
            CloseAndDisposeSerialPort();
            
            if (console.Type == DbConsoleType.Serial)
                AddSerialPort();
        }
        
        public void Start()
        {
            if (Console.Type == DbConsoleType.Serial && SerialPort?.IsOpen == false)
                SerialPort.Open();
        }
        
        public void Stop()
        {
            if (Console.Type == DbConsoleType.Serial && SerialPort?.IsOpen == true)
                SerialPort.Close();
        }

        public bool IsListening()
        {
            if (Console.Type == DbConsoleType.Serial && SerialPort?.IsOpen == true)
                return true;

            return false;
        }

        public void Dispose()
        {
            CloseAndDisposeSerialPort();
        }
    }
    
    public class ConsolesManager
    {
        private readonly List<ConsoleManagerUnit> _units;

        public ConsolesManager()
        {
            _units = new List<ConsoleManagerUnit>();
        }

        public void Add(DbConsole console)
        {
            _units.Add(new ConsoleManagerUnit(console));
        }

        public void Update(DbConsole console)
        {
            var updUnit = _units.FirstOrDefault(u => u.ConsoleId == console.Id);
            updUnit?.Update(console);
        }

        public void Remove(DbConsole console)
        {
            var remUnit = _units.FirstOrDefault(u => u.ConsoleId == console.Id);
            _units.Remove(remUnit);
            remUnit?.Dispose();
        }

        public bool Exist(DbConsole console)
        {
            return _units.Any(u => u.ConsoleId == console.Id);
        }
        
        public List<ConsoleOutput> GetConsoleOutput(DbConsole console, DateTime? start = null)
        {
            List<ConsoleOutput> list = new List<ConsoleOutput>();
            var unit = _units.FirstOrDefault(o => o.ConsoleId == console.Id);
            if (unit != null)
                list = unit.Output;
            if (start != null)
                list = list.Where(l => l.DateUtc > start.Value.ToUniversalTime()).ToList();
            return list;
        }

        public bool IsListening(DbConsole console)
        {
            var unit = _units.FirstOrDefault(o => o.ConsoleId == console.Id);
            return unit != null && unit.IsListening();
        }

        public void StartListening(DbConsole console)
        {
            var unit = _units.FirstOrDefault(o => o.ConsoleId == console.Id);
            unit?.Start();
        }
        
        public void StopListening(DbConsole console)
        {
            var unit = _units.FirstOrDefault(o => o.ConsoleId == console.Id);
            unit?.Stop();
        }
    }
}