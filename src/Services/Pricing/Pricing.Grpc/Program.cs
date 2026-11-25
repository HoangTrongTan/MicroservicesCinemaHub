using Pricing.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//1. DbContext (PostgresSQL)
builder.Services.AddDbContext<PricingDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")!, npgsql =>
    {
        // config cho ef migrate chui vào schema pricing
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "pricing");
    });
});

builder.Services.AddGrpc(options =>
{

});

builder.Services.AddGrpcReflection();

var app = builder.Build();
// Configure the HTTP request pipeline.

// Map endpoint gRPC
app.MapGrpcService<PricingService>();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
} 

//app.MapGrpcService<GreeterService>();

app.Run();

// Add-Migration InitailCreate -OutputDir Migrations