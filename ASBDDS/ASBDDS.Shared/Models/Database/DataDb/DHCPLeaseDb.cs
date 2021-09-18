namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class DHCPLeaseDb : DbBaseModel
    {
        public bool Static { get; set; }
        public string Address { get; set; }
        public string MacAddress { get; set; }
    }
}