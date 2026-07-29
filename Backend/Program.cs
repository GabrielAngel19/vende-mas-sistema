var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/saludo", () =>
{
    return Results.Ok(new
    {
        mensaje = "Hola desde C#"
    });
});

app.Run();