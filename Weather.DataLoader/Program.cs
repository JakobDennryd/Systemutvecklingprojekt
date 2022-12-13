using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Weather.DataLoader.Models;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .AddEnvironmentVariables()
    .Build();

var servicesConfig = config.GetSection("Services");

var tempServiceConfig = servicesConfig.GetSection("Temperature");
var tempServiceHost = tempServiceConfig["Host"];
var tempServicePort = tempServiceConfig["Port"];

var precipServiceConfig = servicesConfig.GetSection("Temperature");
var precipServiceHost = precipServiceConfig["Host"];
var precipServicePort = precipServiceConfig["Port"];


var zipCodes = new List<string>
{
    "73026",
    "68104",
    "04401",
    "32808",
    "19717"
};

Console.WriteLine("Starting Data Load");

var temperatureHttpClient = new HttpClient();
temperatureHttpClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var precipHttpClient = new HttpClient();
precipHttpClient.BaseAddress = new Uri($"http://{precipServiceHost}:{precipServicePort}");

foreach(var code in zipCodes)
{
    Console.WriteLine($"Processing Zip Code: {code}");

    var from = DateTime.Now.AddYears(-2);
    var thru = DateTime.Now;

    for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
    {
        var temps = PostTemp(code, day, temperatureHttpClient);
        PostPrecip(temps[0], code, day, precipHttpClient);
    }
}

List<int> PostTemp(string code, DateTime day, HttpClient temperatureHttpClient)
{
    var rand = new Random();
    var t1 = rand.Next(-10, 30);
    var t2 = rand.Next(-10, 30);
    var hiLoTemps = new List<int> { t1, t2};
    hiLoTemps.Sort();

    var temperatureObservation = new TemperatureModel
    {
        TempLowC = hiLoTemps[0],
        TempHighC = hiLoTemps[1],
        ZipCode = code,
        CreatedOn = day,
    };

    var tempResponse = temperatureHttpClient.PostAsJsonAsync("observation", temperatureObservation).Result;

    if (tempResponse.IsSuccessStatusCode)
    {
        Console.Write($"Posted Temperature: Date: {day:d} " +
            $"Zip: {code} " +
            $"lo (C): {hiLoTemps[0]} " +
            $"Hi (C): {hiLoTemps[1]}");
    }
    else
    {
        Console.WriteLine(tempResponse.ToString());
    }

    return hiLoTemps;
}

void PostPrecip(int lowTemp, string code, DateTime day, HttpClient precipHttpClient)
{
    var rand = new Random();
    var isPrecip = rand.Next(2) < 1;

    PrecipitationModel precipitation;

    if (isPrecip)
    {
        var precipCm = rand.Next(1, 20);
        if(lowTemp < 1)
        {
            precipitation = new PrecipitationModel
            {
                AmountCm = precipCm,
                WeatherType = "snow",
                ZipCode = code,
                CreatedOn = day,
            };
        }
        else
        {
            precipitation = new PrecipitationModel
            {
                AmountCm = precipCm,
                WeatherType = "rain",
                ZipCode = code,
                CreatedOn = day,
            };
        }
    }
    else
    {
        precipitation = new PrecipitationModel
        {
            AmountCm = 0,
            WeatherType = "none",
            ZipCode = code,
            CreatedOn = day,
        };
    }

    

    var precipResponse = precipHttpClient.PostAsJsonAsync("observation", precipitation).Result;

    if (precipResponse.IsSuccessStatusCode)
    {
        Console.Write($"Posted Precipitation: Date {day:d} " +
            $"Zip: {code} " +
            $"Type: {precipitation.WeatherType} " +
            $"Amount (cm.): {precipitation.AmountCm}");
    }
    else
    {
        Console.WriteLine(precipResponse.ToString());
    }
}