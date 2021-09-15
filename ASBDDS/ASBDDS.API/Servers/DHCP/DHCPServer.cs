using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using GitHub.JPMikkers.DHCP;

using DHCPJPMikkers = GitHub.JPMikkers.DHCP;

namespace ASBDDS.API.Servers.DHCP
{
    public enum BootMode
    {
        PXE,
        IPXE,
        HTTP,
        UBOOT,
        OTHER = 99
    }

    public class DHCPServer : DHCPJPMikkers.DHCPServer
    {
        public DHCPServer(IPAddress host, int port, IDHCPLeasesManager leasesManager, List<OptionItem> options) : base(host, port, leasesManager, options)
        {
            bindAddress = host;
            OnStatusChange += _OnStatusChange;
            OnTrace += _OnTrace;
        }

        private IPAddress bindAddress;
        private static void _OnStatusChange(object sender, DHCPJPMikkers.DHCPStopEventArgs e)
        {
            Trace.WriteLine(e?.Reason);
            Trace.Flush();
        }
        private static void _OnTrace(object sender, DHCPJPMikkers.DHCPTraceEventArgs e)
        {
            Trace.WriteLine(e?.Message);
            Trace.Flush();
        }

        protected override void ProcessingReceiveMessage(DHCPMessage sourceMsg, DHCPMessage targetMsg)
        {
            var arch = sourceMsg.GetArch();
            var bootMode = sourceMsg.GetBootMode();
            var bootFile = "uboot.bin";
            switch (bootMode)
            {
                case BootMode.PXE:
                    switch (arch)
                    {
                        case "aarch64":
                            bootFile = "uboot.bin";
                            break;
                    }
                    break;
                case BootMode.IPXE:
                    bootFile = "ipxe.efi.cfg";
                    break;
                case BootMode.HTTP:
                case BootMode.UBOOT:
                    switch (arch)
                    {
                        case "aarch64":
                            bootFile = "arm64-ipxe.efi";
                            break;
                    }
                    break;
                case BootMode.OTHER:
                    break;
            }

            targetMsg.BootFileName = bootFile;
            targetMsg.NextServerIPAddress = bindAddress;
        }
    }

    public static class DHCPMessageExtensions
    {
        // https://www.iana.org/assignments/dhcpv6-parameters/dhcpv6-parameters.xhtml#processor-architecture 
        private static string[] archTypes = {
            "x86 BIOS",
            "NEC/PC98 (DEPRECATED)",
            "Itanium",
            "DEC Alpha (DEPRECATED)",
            "Arc x86 (DEPRECATED)",
            "Intel Lean Client (DEPRECATED)",
            "x86 UEFI",
            "x64 UEFI",
            "EFI Xscale (DEPRECATED)",
            "EBC",
            "ARM 32-bit UEFI",
            "ARM 64-bit UEFI",
            "PowerPC Open Firmware",
            "PowerPC ePAPR",
            "POWER OPAL v3",
            "x86 uefi boot from http",
            "x64 uefi boot from http",
            "ebc boot from http",
            "arm uefi 32 boot from http",
            "arm uefi 64 boot from http",
            "pc/at bios boot from http",
            "arm 32 uboot",
            "arm 64 uboot",
            "arm uboot 32 boot from http",
            "arm uboot 64 boot from http",
            "RISC-V 32-bit UEFI",
            "RISC-V 32-bit UEFI boot from http",
            "RISC-V 64-bit UEFI",
            "RISC-V 64-bit UEFI boot from http",
            "RISC-V 128-bit UEFI",
            "RISC-V 128-bit UEFI boot from http",
            "s390 Basic",
            "s390 Extended",
        };

        public static string GetArch(this DHCPMessage message)
        {
            byte archByte = 0;
            try
            {

                archByte = message.Options
                    .Where(x => x.OptionType == TDHCPOption.ClientSystemArchitectureType)
                    .Cast<DHCPOptionGeneric>()
                    .Select(x => x.Data[1])
                    .First();
            }
            catch {}

            // Rpi4 bootloader not set ClientSystemArchitectureType DHCP request option, but we have 97 DHCP option for determinate arch, model, serial and mac address.
            if (archByte == 0)
            {
                var clientIdentifier = message.Options
                    .Where(x => x.OptionType == (TDHCPOption)97)
                    .Cast<DHCPOptionGeneric>().FirstOrDefault();
                if (clientIdentifier != null)
                {
                    var asciiClientIdentifier = Encoding.ASCII.GetString(clientIdentifier.Data);
                    if (asciiClientIdentifier.Contains("rpi", StringComparison.OrdinalIgnoreCase))
                        archByte = 0x16; // arm 64 uboot
                }
            }
            var archType = archTypes[archByte];
            var arch = "amd64";

            if (archType.Contains("arm", StringComparison.OrdinalIgnoreCase))
            {
                if (archType.Contains("64", StringComparison.OrdinalIgnoreCase))
                    arch = "aarch64";
                else
                    arch = "arm";
            }

            return arch;
        }

        public static string GetVendorClass(this DHCPMessage message)
        {
            var sb = new StringBuilder();
            var strings = message.Options
             .Where(x => x.OptionType == TDHCPOption.VendorClassIdentifier)
             .Cast<DHCPOptionVendorClassIdentifier>()
             .Select(x => Encoding.ASCII.GetString(x.Data));

            try
            {
                foreach (var s in strings)
                {
                    sb.AppendLine(s);
                }
            }
            catch { }
            return sb.ToString();
        }

        public static BootMode GetBootMode(this DHCPMessage message)
        {
            BootMode bootMode;
            
            if (IsIPXE(message))
                bootMode = BootMode.IPXE;
            else if (IsPXE(message))
                bootMode = BootMode.PXE;
            else if (IsHTTP(message))
                bootMode = BootMode.HTTP;
            else if (IsUBoot(message))
                bootMode = BootMode.UBOOT;
            else
                bootMode = BootMode.OTHER;

            return bootMode;
        }
        
        public static bool IsHTTP(this DHCPMessage message) => GetVendorClass(message).Contains("HTTPClient");
        public static bool IsPXE(this DHCPMessage message) => GetVendorClass(message).Contains("PXEClient");
        public static bool IsUBoot(this DHCPMessage message) => GetVendorClass(message).Contains("u-boot", StringComparison.OrdinalIgnoreCase);
        public static bool IsIPXE(this DHCPMessage message)
        {
            try
            {
                return message.Options
                    .Where(x => x.OptionType == (TDHCPOption)77)
                    .Cast<DHCPOptionGeneric>()
                    .Select(x => Encoding.ASCII.GetString(x.Data))
                    .Any(x => x.Equals("iPXE", StringComparison.InvariantCultureIgnoreCase));
            }
            catch { return false; }
        }
    }
}
