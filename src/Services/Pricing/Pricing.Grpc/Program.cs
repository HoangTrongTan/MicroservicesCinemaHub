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

builder.Services.AddGrpc();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
} 

//app.MapGrpcService<GreeterService>();

app.Run();

// Add-Migration InitailCreate -OutputDir Data/Migrations