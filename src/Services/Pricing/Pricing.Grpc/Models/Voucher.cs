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
}