namespace Pricing.Grpc.Data.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PricingDbContext>();

        await context.Database.MigrateAsync();
        await SeedVouchersAsync(context);
    }

    private static async Task SeedVouchersAsync(PricingDbContext context)
    {
        if (await context.Vouchers.AnyAsync()) return;

        await context.Vouchers.AddRangeAsync(InitialData.Vouchers);
        await context.SaveChangesAsync();
    }
}