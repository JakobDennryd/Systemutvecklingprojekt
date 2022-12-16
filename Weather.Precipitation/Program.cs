using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather.Precipitation.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PrecipDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"));
}
);

var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip, [FromQuery] int? days, PrecipDbContext db) =>
{
    if (days == null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide number of days between 1 and 30");
    }
    var dateStart = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await db.Precipitation
    .Where(precip => precip.ZipCode == zip && precip.CreatedOn > dateStart).ToListAsync();

    return Results.Ok(results);


});

app.MapPost("/observation", async (Precipitation precip, PrecipDbContext db) =>
{
    precip.CreatedOn = precip.CreatedOn.ToUniversalTime();
    await db.AddAsync(precip);
    await db.SaveChangesAsync();
});


app.Run();
