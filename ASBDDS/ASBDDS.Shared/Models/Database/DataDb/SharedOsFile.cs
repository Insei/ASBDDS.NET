namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class SharedOsFile : SharedFile
    {
        public virtual OperationSystemModel Os { get; set; }
    }
}