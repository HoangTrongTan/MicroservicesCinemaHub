namespace Pricing.Grpc.Data.Configurations;

public class VoucherUsageConfiguration : IEntityTypeConfiguration<VoucherUsage>
{
    public void Configure(EntityTypeBuilder<VoucherUsage> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Voucher)
            .WithMany(v => v.Usages)
            .HasForeignKey(u => u.VoucherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => new { u.VoucherId, u.CustomerId })
            .IsUnique()
            .HasFilter("\"IsReleased\" = false");

        builder.HasIndex(u => new { u.VoucherId, u.BookingId })
            .IsUnique();
    }
}
