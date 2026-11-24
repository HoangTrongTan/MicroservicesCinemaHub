namespace Pricing.Grpc.Data.Extensions;

internal class InitialData
{
    public static IEnumerable<Voucher> Vouchers =>
        new List<Voucher>
        {
            Voucher.Create(
                id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                code: "CINEMA20",
                description: "Giảm 20% tối đa 50k cho đơn từ 100k",
                discountType: DiscountType.Percentage,
                discountValue: 20,
                minOrderAmount: 100_000,
                maxDiscountAmount: 50_000,
                expiresAt: new DateTimeOffset(2027, 12, 31, 23, 59, 59, TimeSpan.Zero),
                maxUsageCount: 100),

            Voucher.Create(
                id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
                code: "GIAM50K",
                description: "Giảm thẳng 50k cho đơn từ 200k",
                discountType: DiscountType.FixedAmount,
                discountValue: 50_000,
                minOrderAmount: 200_000,
                maxDiscountAmount: null,
                expiresAt: new DateTimeOffset(2027, 12, 31, 23, 59, 59, TimeSpan.Zero),
                maxUsageCount: 50),

            Voucher.Create(
                id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
                code: "HETHAN10",
                description: "Mã hết hạn để test trường hợp fail",
                discountType: DiscountType.Percentage,
                discountValue: 10,
                minOrderAmount: 0,
                maxDiscountAmount: 20_000,
                expiresAt: new DateTimeOffset(2025, 01, 01, 0, 0, 0, TimeSpan.Zero),
                maxUsageCount: 10),
        };
}