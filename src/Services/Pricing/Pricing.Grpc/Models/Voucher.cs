namespace Pricing.Grpc.Models;
public enum DiscountType
{
    Percentage = 1,   // Giảm theo %
    FixedAmount = 2   // Giảm tiền mặt trực tiếp
}
public class Voucher
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public int MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public bool IsActive { get; set; } = true;

    public List<VoucherUsage> Usages { get; set; } = [];

    public static Voucher Create(
        string code,
        string description,
        DiscountType discountType,
        decimal discountValue,
        decimal minOrderAmount,
        decimal? maxDiscountAmount,
        DateTimeOffset expiresAt,
        int maxUsageCount,
        Guid? id = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(discountValue);
        ArgumentOutOfRangeException.ThrowIfNegative(minOrderAmount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxUsageCount);

        if (discountType == DiscountType.Percentage && discountValue > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountValue), "Giảm phần trăm mà > 100% là định tặng tiền cho khách luôn hả sếp? 😂");
        }

        return new Voucher
        {
            Id = id ?? Guid.NewGuid(),
            Code = code.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            DiscountType = discountType,
            DiscountValue = discountValue,
            MinOrderAmount = minOrderAmount,
            MaxDiscountAmount = discountType == DiscountType.Percentage ? maxDiscountAmount : null,
            ExpiresAt = expiresAt.ToUniversalTime(), // Ép về UTC cho Npgsql đỡ khóc 🐘
            MaxUsageCount = maxUsageCount,
            CurrentUsageCount = 0,
            IsActive = true
        };
    }
}