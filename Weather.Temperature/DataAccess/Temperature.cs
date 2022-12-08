namespace Weather.Temperature.DataAccess
{
    public class Temperature
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal TempHighC { get; set; }
        public decimal TempLowC { get; set; }
        public string ZipCode { get; set; }
    }
}
