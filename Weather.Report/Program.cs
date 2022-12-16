using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather.Report.Config;
using Weather.Report.DataAccess;
using Weather.Report.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IReportAggregator, ReportAggregator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

builder.Services.AddDbContext<WeatherReportDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"));
}
);

var app = builder.Build();

app.MapGet("/weather-report/{zip}", 
    async (string zip, [FromQuery] int? days, IReportAggregator aggregator) =>
{
    if(days == null || days > 30 || days < 1)
    {
        return Results.BadRequest("Please provide a 'days' parameter value between 1 and 30"  );
    }
    var report = await aggregator.BuildReport(zip, days.Value);

    return Results.Ok(report);
});

app.Run();
