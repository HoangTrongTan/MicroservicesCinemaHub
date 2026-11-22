namespace Pricing.Grpc.Models;

public class VoucherUsage
{
    public Guid Id { get; set; }
    public Guid VoucherId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BookingId { get; set; } // Gắn BookingId để Saga biết đường gọi ReleaseVoucher & chống trùng (Idempotency)
    public DateTimeOffset RedeemedAt { get; set; }
    public bool IsReleased { get; set; } 
    public DateTimeOffset? ReleasedAt { get; set; }

    public Voucher Voucher { get; set; } = default!;
}