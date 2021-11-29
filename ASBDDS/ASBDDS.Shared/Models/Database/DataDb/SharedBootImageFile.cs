namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class SharedBootableImageFile : SharedFile
    {
        public virtual BootableImage BootableImage { get; set; }
    }
}