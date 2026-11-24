namespace Pricing.Grpc.Data.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.HasKey(v => v.Id);
        builder.HasIndex(v => v.Code).IsUnique();
        builder.Property(v => v.Code).HasMaxLength(50).IsRequired();
        builder.Property(v => v.DiscountValue).HasPrecision(18, 2);
        builder.Property(v => v.MinOrderAmount).HasPrecision(18, 2);
        builder.Property(v => v.MaxDiscountAmount).HasPrecision(18, 2);

        // uint khôg nhận số âm
        // cột xmin có sẵn trong mọi table => postgress hỗ trợ
        // khi insert, update tự tăng số này lên
        builder.Property<uint>("xmin").IsRowVersion();
    }
}