using ASBDDS.Shared.Models.Database.DataDb;
using Renci.SshNet;
using System;
using System.IO;
using System.Threading;
using ASBDDS.Shared.Interfaces;

namespace ASBDDS.API.Models
{
    public enum UniFiSwitchPOEPortOpmode
    {
        SHUTDOWN,
        PASSIVE24V,
        AUTO
    }

    public class UniFiSwitch : IControlPOESwitchPort
    {
        private void SendSshCommand(SshClient client, string commandsStr)
        {
            client.Connect();
            ShellStream shell = client.CreateShellStream("master", 80, 24, 800, 600, 1024);
            var reader = new StreamReader(shell);
            var writer = new StreamWriter(shell);
            writer.AutoFlush = true;
            foreach (var commandStr in commandsStr.Split(";", StringSplitOptions.RemoveEmptyEntries))
            {
                writer.WriteLine(commandStr);
                while (shell.Length == 0)
                {
                    Thread.Sleep(500);
                }
                Console.WriteLine(reader.ReadToEnd());
            }
            shell.Close();
            client.Disconnect();
        }

        private void SetupPoePortOpmode(SshClient client, SwitchPort port, UniFiSwitchPOEPortOpmode opmode)
        {
            SendSshCommand(client, "telnet localhost; enable; config; interface " + port.Number + "; poe opmode " + opmode.ToString().ToLower() + "; exit; exit; exit; exit; exit;");
        }

        public void EnablePOEPort(SwitchPort port)
        {
            if (port.Switch.AuthMethod == SwitchAuthMethod.SSH_USER_PASS)
            {
                var ConnNfo = new ConnectionInfo(port.Switch.Ip, 22, port.Switch.Username,
                    new AuthenticationMethod[]{
                        new PasswordAuthenticationMethod(port.Switch.Username, port.Switch.Password),
                    }
                );
                using var client = new SshClient(ConnNfo);
                SetupPoePortOpmode(client, port, UniFiSwitchPOEPortOpmode.AUTO);
            }
            else
                throw new NotImplementedException("AuthMethod is not implemented");
        }

        public void DisablePOEPort(SwitchPort port)
        {
            if (port.Switch.AuthMethod == SwitchAuthMethod.SSH_USER_PASS)
            {
                using (var client = new SshClient(port.Switch.Ip, port.Switch.Username, port.Switch.Password))
                {
                    SetupPoePortOpmode(client, port, UniFiSwitchPOEPortOpmode.SHUTDOWN);
                }
            }
            else
                throw new NotImplementedException("AuthMethod is not implemented");
        }
    }
}