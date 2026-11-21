using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Add Service to the container
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddMartenDatabase(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Add the http request pipeline
app.MapCarter();

app.Run();
