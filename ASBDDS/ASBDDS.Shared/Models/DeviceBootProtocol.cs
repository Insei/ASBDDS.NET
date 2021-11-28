namespace ASBDDS.Shared.Models.Database.DataDb
{
    public enum DeviceBootProtocol
    {
        PXE = 0,
        PXE_UEFI = 5,
        IPXE = 10,
        Legacy = 15,
        UEFI = 20
    }
}