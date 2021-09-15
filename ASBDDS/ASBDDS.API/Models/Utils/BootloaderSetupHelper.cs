using ASBDDS.Shared.Helpers;
using ASBDDS.Shared.Models.Database.DataDb;
using System;
using System.IO;

namespace ASBDDS.API.Models.Utils
{
    public static class BootloaderSetupHelper
    {
        public static readonly string TftpDirectory = Path.Combine(Environment.CurrentDirectory, "tftp_root");
        public static readonly string ImagesDirectory = Path.Combine(Environment.CurrentDirectory, "images");

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void MakeFirmware(Device device)
        {
            var deviceHelper = new DeviceHelper();
            var firmwarePath = Path.Combine(ImagesDirectory, deviceHelper.GetSystemBaseModel(device.Model), "firmware");
            var outPath = Path.Combine(TftpDirectory, device.MacAddress);
            CopyAll(new DirectoryInfo(firmwarePath), new DirectoryInfo(outPath));
        }

        public static void MakeUboot(Device device, string variant = "normal")
        {
            var deviceHelper = new DeviceHelper();
            var ubootPath = Path.Combine(ImagesDirectory, deviceHelper.GetSystemBaseModel(device.Model), "u-boot", variant);
            var outPath = Path.Combine(TftpDirectory, device.MacAddress);
            CopyAll(new DirectoryInfo(ubootPath), new DirectoryInfo(outPath));
        }

        public static void MakeIpxe(Device device, string ipxeUrl)
        {
            var ipxePath = Path.Combine(ImagesDirectory, "ipxe");
            var outPath = Path.Combine(TftpDirectory, device.MacAddress);
            CopyAll(new DirectoryInfo(ipxePath), new DirectoryInfo(outPath));
            var ixpeStr = "";
            using (var reader = File.OpenText(Path.Combine(outPath , "ipxe.efi.cfg")))
            {
                ixpeStr = reader.ReadToEnd();
            }
            ixpeStr = ixpeStr.Replace("REPLACEURL", ipxeUrl);            
          
            using (var file = new StreamWriter(Path.Combine(outPath , "ipxe.efi.cfg")))
            {
                file.Write(ixpeStr);
            }
        }
        

        public static void RemoveDeviceDirectory(Device device)
        {
            Directory.Delete(Path.Combine(TftpDirectory, device.MacAddress), true);
        }
    }
}
