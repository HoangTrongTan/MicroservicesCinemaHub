namespace Pricing.Grpc.Services;

internal static class PricingCalculator
{
    public static decimal GetTimeSlotMultiplier(DateTimeOffset showtimeVn)
    {
        decimal multiplier = 1.0m;

        if (showtimeVn.Hour < 12) multiplier *= 0.8m; // Trước 12h giảm 20%
        else if (showtimeVn.Hour >= 22) multiplier *= 0.9m; // Sau 22h giảm 10%

        if (showtimeVn.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            multiplier *= 1.15m;
        }

        return Math.Round(multiplier, 4);
    }
    public static decimal CalculateVoucherDiscount(Voucher voucher, decimal orderAmount)
    {
        var discount = voucher.DiscountType == DiscountType.Percentage
            ? Math.Round(orderAmount * (voucher.DiscountValue / 100m), 0)
            : voucher.DiscountValue;

        if (voucher.MaxDiscountAmount.HasValue)
            discount = Math.Min(discount, voucher.MaxDiscountAmount.Value);

        return Math.Min(discount, orderAmount);
    }
    public static decimal GetSeatTypeMultiplier(SeatTypeProto type) => type switch
    {
        SeatTypeProto.Vip => 1.3m,
        SeatTypeProto.Couple => 2.2m,
        _ => 1.0m
    };
    public static decimal GetMembershipDiscountRate(MembershipTierProto tier) => tier switch
    {
        MembershipTierProto.Silver => 0.05m,
        MembershipTierProto.Gold => 0.10m,
        MembershipTierProto.Platinum => 0.15m,
        _ => 0m
    };
}