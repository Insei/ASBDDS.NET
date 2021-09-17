using ASBDDS.Shared.Models.Database.DataDb;
using Renci.SshNet;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ASBDDS.Shared.Interfaces;

namespace ASBDDS.API.Models
{
    public enum UniFiSwitchPOEPortOpmode
    {
        SHUTDOWN,
        PASSIVE24V,
        AUTO
    }

    public class UniFiSwitch : IDevicePowerControl
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

        private int SetupPoePortOpmode(SshClient client, SwitchPort port, UniFiSwitchPOEPortOpmode opmode)
        {
            SendSshCommand(client, "telnet localhost; enable; config; interface " + port.Number + "; poe opmode " + opmode.ToString().ToLower() + "; exit; exit; exit; exit; exit;");
            return 0;
        }

        public async Task<int> PowerOn(Device device, CancellationToken cancellationToken = default)
        {
            var port = device.SwitchPort;
            if (port.Switch.AuthMethod == SwitchAuthMethod.SSH_USER_PASS)
            {
                var connInfo = new ConnectionInfo(port.Switch.Ip, 22, port.Switch.Username,
                    new AuthenticationMethod[]{
                        new PasswordAuthenticationMethod(port.Switch.Username, port.Switch.Password),
                    }
                );
                using var client = new SshClient(connInfo);
                return await Task.Run(() => SetupPoePortOpmode(client, port, UniFiSwitchPOEPortOpmode.AUTO), cancellationToken);
            }
            throw new NotImplementedException("AuthMethod is not implemented");
        }

        public async Task<int> PowerOff(Device device, CancellationToken cancellationToken = default)
        {
            var port = device.SwitchPort;
            if (port.Switch.AuthMethod == SwitchAuthMethod.SSH_USER_PASS)
            {
                using (var client = new SshClient(port.Switch.Ip, port.Switch.Username, port.Switch.Password))
                {
                    return await Task.Run(() => SetupPoePortOpmode(client, port, UniFiSwitchPOEPortOpmode.SHUTDOWN), cancellationToken);
                }
            }
            throw new NotImplementedException("AuthMethod is not implemented");
        }
    }
}