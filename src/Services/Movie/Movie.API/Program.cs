var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Add Service to the container
builder.Services.AddStackExchangeRedisCache(options =>
{   
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MoviesApi_";
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    config.AddOpenBehavior(typeof(CachingBehavior<,>));
    config.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
});

builder.Services.AddMartenDatabase(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Add the http request pipeline
app.UseExceptionHandler(opts => { });

app.MapCarter();

app.Run();

//docker run -d --name cinema-redis -p 6379:6379 redis: 7 - alpine redis - server--maxmemory 128mb --maxmemory-policy allkeys-lru