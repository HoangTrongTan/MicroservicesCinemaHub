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

// 2. gRPC + Exception Interceptor + Reflection 
// AddGrpcReflection để postman đỡ phải import tay
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});
builder.Services.AddGrpcReflection();

builder.Services.AddGrpcHealthChecks()
    .AddCheck("PricingService", () => HealthCheckResult.Healthy())
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

// Map endpoint gRPC
app.MapGrpcService<PricingService>();
app.MapGrpcHealthChecksService();

// Để postman tự động soi danh sách RPC kh cần import tay.
app.MapGrpcReflectionService();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

// Add-Migration InitailCreate -OutputDir Migrations