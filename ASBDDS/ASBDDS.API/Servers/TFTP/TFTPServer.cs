﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tftp.Net;

namespace ASBDDS.API.Servers.TFTP
{
    public class TFTPServer
    {
        public string RootDirectory = Environment.CurrentDirectory;
        public string TftpDirectory { get; }
        private TftpServer server { get; set; }
        public TFTPServer(string tftp_dir)
        {
            server = new TftpServer();
            TftpDirectory = Path.Combine(RootDirectory, tftp_dir);
            CreateTftpRootPath();
        }

        private void CreateTftpRootPath()
        {
            try
            {
                if (Directory.Exists(TftpDirectory))
                {
                    return;
                }
                DirectoryInfo di = Directory.CreateDirectory(TftpDirectory);

            }
            catch (Exception e) { }
            finally { }
        }

        public void Start()
        {
            server.OnWriteRequest += new TftpServerEventHandler(OnWriteRequest);
            server.OnReadRequest += new TftpServerEventHandler(OnReadRequest);
            server.Start();
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
            String path = Path.Combine(TftpDirectory, transfer.Filename);
            FileInfo file = new FileInfo(path);

            //Is the file within the server directory?
            if (!file.FullName.StartsWith(RootDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
            }
            else if (!file.Exists)
            {
                CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
            }
            else
            {
                using (var stream = new FileStream(file.FullName, FileMode.Open))
                {
                    StartTransfer(transfer, stream);
                }
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