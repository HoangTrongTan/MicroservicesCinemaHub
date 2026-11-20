var builder = WebApplication.CreateBuilder(args);
// Add Service to the container

var app = builder.Build();
// Add the http request pipeline

app.MapGet("/", () => "Hello World!");

app.Run();
