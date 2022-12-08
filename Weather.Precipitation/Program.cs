using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/obeservation/{zip}", (string zip, [FromQuery] int? days)=>
{
    return Results.Ok(zip);
});


app.Run();
