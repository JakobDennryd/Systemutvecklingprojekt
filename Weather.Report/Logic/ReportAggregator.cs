using Microsoft.Extensions.Options;
using System.Text.Json;
using Weather.Report.Config;
using Weather.Report.DataAccess;
using Weather.Report.Models;

namespace Weather.Report.Logic
{

    public interface IReportAggregator
    {
        public Task<WeatherReport> BuildReport(string zip, int days);
    }
    public class ReportAggregator : IReportAggregator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReportAggregator> _logger;
        private readonly WeatherDataConfig _weatherDataConfig;
        private readonly WeatherReportDbContext _ctx;

        public ReportAggregator(
            IHttpClientFactory http,
            ILogger<ReportAggregator> logger,
            IOptions<WeatherDataConfig> weatherConfig,
            WeatherReportDbContext ctx)
        {
            _httpClientFactory = http;
            _logger = logger;
            _weatherDataConfig = weatherConfig.Value;
            _ctx = ctx;
        }

        public async Task<WeatherReport> BuildReport(string zip, int days)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var precipData = await FetchPrecipData(httpClient, zip, days);
           

            var totalSnow = GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);
            _logger.LogInformation($"zip: {zip} over last {days} days: " + $"total snow: {totalSnow}, rain: {totalRain}");

            var temperatureData = await FetchTemperatureData(httpClient, zip, days);

            var averageHigh = temperatureData.Average(t => t.TempHighC);
            var averageLow = temperatureData.Average(t => t.TempLowC);

            _logger.LogInformation($"zip: {zip} over last {days} days: " + $"average high: {averageHigh}°C, average low: {averageLow}°C");


            var report = new WeatherReport
            {
                AverageHighC = averageHigh,
                AverageLowC = averageLow,
                RainfallTotalCm = totalRain,
                SnowTotalCm = totalSnow,
                ZipCode = zip,
                CreatedOn = DateTime.UtcNow,
            };

            await _ctx.AddAsync(report);
            await _ctx.SaveChangesAsync();

            return report;

        }

        private static decimal GetTotalSnow(IEnumerable<PrecipitationModel> data)
        {
            var totalSnow = data.Where(p => p.WeatherType == "snow").Sum(p => p.AmountCm);

            return Math.Round(totalSnow, 1);
        }

        private static decimal GetTotalRain(IEnumerable<PrecipitationModel> data)
        {
            var totalRain = data.Where(p => p.WeatherType == "rain").Sum(p => p.AmountCm);

            return Math.Round(totalRain, 1);
        }

        private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildTemperatureEndPoint(zip, days);
            var records = await httpClient.GetAsync(endpoint);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var data = await records.Content.ReadFromJsonAsync<List<TemperatureModel>>(jsonSerializerOptions);
            return data ?? new List<TemperatureModel>(); 
        }

     

        private async Task<List<PrecipitationModel>> FetchPrecipData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildPrecipitationEndPoint(zip, days);
            var records = await httpClient.GetAsync(endpoint);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var data = await records.Content.ReadFromJsonAsync<List<PrecipitationModel>>(jsonSerializerOptions);
            return  data ?? new List<PrecipitationModel>();
        }




        private string BuildTemperatureEndPoint(string zip, int days)
        {
            var protocol = _weatherDataConfig.TempDataProtocol;
            var host = _weatherDataConfig.TempDataHost;
            var port = _weatherDataConfig.TempDataPort;

            return $"{host}/observation/{zip}?days={days}";
        }

        private string BuildPrecipitationEndPoint(string zip, int days)
        {
            var protocol = _weatherDataConfig.PrecipDataProtocol;
            var host = _weatherDataConfig.PrecipDataHost;
            var port = _weatherDataConfig.PrecipDataPort;

            return $"{host}/observation/{zip}?days={days}";
        }
    }
}
