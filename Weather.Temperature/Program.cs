using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather.Temperature.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TemperatureDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"));
}
);

var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip, [FromQuery] int? days, TemperatureDbContext db) =>
{
    if (days == null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide number of days between 1 and 30");
    }
    var dateStart = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await db.Temperature
    .Where(precip => precip.ZipCode == zip && precip.CreatedOn > dateStart).ToListAsync();

    return Results.Ok(results);


});

app.MapPost("/observation", async ([FromBody] Temperature temperature, TemperatureDbContext db) =>
{
    await db.AddAsync(temperature);
    await db.SaveChangesAsync();
});

app.Run();
