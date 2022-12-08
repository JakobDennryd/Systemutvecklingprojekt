namespace Weather.Report.DataAccess
{
    public class WeatherReport
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal AverageHighC { get; set; }
        public decimal AverageLowC { get; set; }
        public decimal RainfallTotalCm { get; set; }
        public decimal SnowTotalCm { get; set; }
        public string ZipCode { get; set; }
    }
}
