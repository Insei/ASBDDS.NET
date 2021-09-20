using ASBDDS.API.Models.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ASBDDS.API.Servers.DHCP;
using Tftp.Net;

namespace ASBDDS.API.Servers.TFTP
{
    public class TFTPServer
    {
        public string TftpDirectory { get; }
        private readonly TftpServer _tftp;
        private DHCPServer _dhcp;

        public TFTPServer(string ip, int port = 69, DHCPServer dhcp = null)
        {
            IPAddress ipAddr = IPAddress.Any;
            if(!string.IsNullOrEmpty(ip))
                ipAddr = IPAddress.Parse(ip);
            
            _tftp = new TftpServer(ipAddr, port);
            _dhcp = dhcp;
            TftpDirectory = BootloaderSetupHelper.TftpDirectory;
            CreateTftpRootPath();
        }

        private void CreateTftpRootPath()
        {
            try
            {
                if (Directory.Exists(TftpDirectory))
                    return;
                DirectoryInfo di = Directory.CreateDirectory(TftpDirectory);
            }
            catch { }
        }

        public void Start()
        {
            _tftp.OnWriteRequest += OnWriteRequest;
            _tftp.OnReadRequest += OnReadRequest;
            _tftp.Start();
        }

        private void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
        {
            transfer.Cancel(reason);
        }

        private void OnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {
            CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
        }

        private void OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            // For now we save bootloaders files to mac address folder in tftp directory, and switch bootloaders in this directory if this needed.
            var ipEndPointClient = client as IPEndPoint;
            var macAddress = _dhcp?.Leases.FirstOrDefault(l => l.Address.Equals(ipEndPointClient?.Address))?.MacAddress;
            if (string.IsNullOrEmpty(macAddress))
            {
                CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
                return;
            }

            var path = Path.Combine(TftpDirectory, macAddress, transfer.Filename);
            var file = new FileInfo(path);

            //Is the file within the server directory?
            if (!file.FullName.StartsWith(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
            }
            else if (!file.Exists)
            {
                CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
            }
            else
            {
                var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                StartTransfer(transfer, stream);
            }
        }

        private void StartTransfer(ITftpTransfer transfer, Stream stream)
        {
            //transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            //transfer.OnError += new TftpErrorHandler(transfer_OnError);
            //transfer.OnFinished += new TftpEventHandler(transfer_OnFinished);
            transfer.Start(stream);
        }
    }
}
