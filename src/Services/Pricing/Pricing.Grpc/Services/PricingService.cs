using Grpc.Core;

namespace Pricing.Grpc.Services;

public class PricingService
    (PricingDbContext dbContext, ILogger<PricingService> logger)
    : PricingProtoService.PricingProtoServiceBase
{
    public override async Task<CalculatePriceResponse> CalculatePrice(CalculatePriceRequest request, ServerCallContext context)
    {
        if (request.BasePrice <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Giá cơ bản phải > 0 nha sếp!"));
        }

        if (request.Seats.Count == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Phải chọn ít nhất 1 ghế chứ!"));
        }

        var basePrice = (decimal)request.BasePrice;
        // Quy đổi sang giờ VN (UTC+7) để check khung giờ trước 12h / sau 22h / cuối tuần cho chuẩn
        var showtimeVn = request.ShowtimeStart.ToDateTimeOffset().ToOffset(TimeSpan.FromHours(7));
        var timeSlotMultiplier = PricingCalculator.GetTimeSlotMultiplier(showtimeVn);

        var response = new CalculatePriceResponse();
        decimal subtotal = 0;

        // Tầng 1 & 2: Loại ghế + Khung giờ
        foreach (var seat in request.Seats)
        {
            var seatMultiplier = PricingCalculator.GetSeatTypeMultiplier(seat.SeatType);
            var seatFinal = Math.Round(basePrice * seatMultiplier * timeSlotMultiplier, 0);
            subtotal += seatFinal;

            response.SeatBreakdowns.Add(new SeatPriceBreakdown
            {
                SeatCode = seat.SeatCode,
                SeatType = seat.SeatType,
                BasePrice = (double)basePrice,
                SeatTypeMultiplier = (double)seatMultiplier,
                TimeSlotMultiplier = (double)timeSlotMultiplier,
                SeatFinalPrice = (double)seatFinal,
            });

        }

        // Tầng 3: Chiết khấu hạng thành viên
        var membershipRate = PricingCalculator.GetMembershipDiscountRate(request.MembershipTier);
        var membershipDiscount = Math.Round(subtotal * membershipRate, 0);
        var afterMembership = subtotal - membershipDiscount;

        // Tầng 4: Voucher(nếu có)
        decimal voucherDiscount = 0;
        var appliedVoucher = string.Empty;

        if (!string.IsNullOrWhiteSpace(request.VoucherCode))
        {
            Guid.TryParse(request.CustomerId, out var customerId);
            var (isValid, _, discount) = await EvaluateVoucherAsync(
                request.VoucherCode, 
                customerId, 
                eligibilityAmount: subtotal,
                context.CancellationToken,
                discountBaseAmount: afterMembership);

            if (isValid)
            {
                voucherDiscount = discount;
                appliedVoucher = request.VoucherCode.Trim().ToUpperInvariant();
            }
        }

        var finalTotal = Math.Max(0, afterMembership - voucherDiscount);

        response.Subtotal = (double)subtotal;
        response.MembershipDiscount = (double)membershipDiscount;
        response.VoucherDiscount = (double)voucherDiscount;
        response.FinalTotal = (double)finalTotal;
        response.AppliedVoucherCode = appliedVoucher;

        return response;
    }
    public override async Task<ValidateVoucherResponse> ValidateVoucher(ValidateVoucherRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.CustomerId, out var customerId);
        var (isValid, message, discount) = await EvaluateVoucherAsync(
            request.VoucherCode, 
            customerId,
            eligibilityAmount: (decimal)request.OrderAmount,
            ct: context.CancellationToken);

        return new ValidateVoucherResponse
        {
            IsValid = isValid,
            Message = message,
            EstimatedDiscount = (double)discount,
        };
    }
    public override async Task<RedeemVoucherResponse> RedeemVoucher(RedeemVoucherRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.CustomerId, out var customerId) ||
            !Guid.TryParse(request.BookingId, out var bookingId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "CustomerId hoặc BookingId không phải Guid hợp lệ!"));
        }

        var code = request.VoucherCode.Trim().ToUpperInvariant();
        var voucher = await dbContext.Vouchers
            .FirstOrDefaultAsync(v => v.Code == code, context.CancellationToken);

        if (voucher is null)
            return new RedeemVoucherResponse { Success = false, Message = "Mã voucher không tồn tại!" };

        // Idempotency: Saga lỡ gửi trùng cùng BookingId thì trả OK luôn, không trừ 2 lần.
        var existingUsage = await dbContext.VoucherUsages
            .FirstOrDefaultAsync(u => u.VoucherId == voucher.Id && u.BookingId == bookingId, context.CancellationToken);

        if (existingUsage is { IsReleased: false })
        {
            var existingDiscount = PricingCalculator.CalculateVoucherDiscount(voucher, (decimal)request.OrderAmount);
            return new RedeemVoucherResponse
            {
                Success = true,
                DiscountAmount = (double)existingDiscount,
                Message = "Voucher đã được áp dụng cho booking này trước đó (Idempotent)."
            };
        }

        var (isValid, message, discount) = await EvaluateVoucherAsync(
            voucher, customerId, (decimal)request.OrderAmount, context.CancellationToken);

        if (!isValid)
            return new RedeemVoucherResponse { Success = false, Message = message };

        voucher.CurrentUsageCount++;
        dbContext.VoucherUsages.Add(new VoucherUsage
        {
            Id = Guid.NewGuid(),
            VoucherId = voucher.Id,
            CustomerId = customerId,
            BookingId = bookingId,
            RedeemedAt = DateTimeOffset.UtcNow,
            IsReleased = false
        });

        await dbContext.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Đã redeem voucher {Code} cho Booking {BookingId}", code, bookingId);

        return new RedeemVoucherResponse
        {
            Success = true,
            DiscountAmount = (double)discount,
            Message = "Áp dụng voucher thành công!"
        };
    }
    public override async Task<ReleaseVoucherResponse> ReleaseVoucher(ReleaseVoucherRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.BookingId, out var bookingId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "BookingId không hợp lệ!"));
        }

        var code = request.VoucherCode.Trim().ToUpperInvariant();
        var usage = await dbContext.VoucherUsages
            .Include(u => u.Voucher)
            .FirstOrDefaultAsync(u => u.BookingId == bookingId && u.Voucher.Code == code);

        if (usage is null || usage.IsReleased)
        {
            return new ReleaseVoucherResponse
            {
                Success = true,
                Message = "Voucher chưa dùng hoặc đã được hoàn lại trước đó rồi."
            };
        }

        usage.IsReleased = true;
        usage.ReleasedAt = DateTimeOffset.UtcNow;
        usage.Voucher.CurrentUsageCount = Math.Max(0, usage.Voucher.CurrentUsageCount - 1);

        await dbContext.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("🔄 [COMPENSATION] Đã hoàn trả voucher {Code} cho Booking {BookingId}", code, bookingId);

        return new ReleaseVoucherResponse
        {
            Success = true,
            Message = "Đã giải phóng voucher thành công!"
        };
    }
    private async Task<(bool IsValid, string Message, decimal Discount)> EvaluateVoucherAsync(
        string rawCode, Guid customerId, decimal eligibilityAmount, CancellationToken ct, decimal? discountBaseAmount = null)
    {
        var code = rawCode.Trim().ToUpperInvariant(); //Đổi sang chữ chuẩn chung quốc tế thay vì khác nhau mỗi vùng miền!.
        var voucher = await dbContext.Vouchers
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Code == code, ct);

        if (voucher is null) return (false, "Không tìm thấy mã voucher!", 0);
        return await EvaluateVoucherAsync(voucher, customerId, eligibilityAmount, ct, discountBaseAmount);
    }
    private async Task<(bool IsValid, string Message, decimal Discount)> EvaluateVoucherAsync(
        Voucher voucher, Guid customerId, decimal eligibilityAmount, CancellationToken ct, decimal? discountBaseAmount = null)
    {
        if (!voucher.IsActive) return (false, "Voucher đã bị vô hiệu hóa!", 0);
        if (voucher.ExpiresAt <= DateTimeOffset.UtcNow) return (false, "Voucher đã hết hạn rồi sếp ơi!", 0);
        if (voucher.CurrentUsageCount >= voucher.MaxUsageCount) return (false, "Voucher đã hết lượt sử dụng!", 0);
        if (eligibilityAmount < voucher.MinOrderAmount)
            return (false, $"Đơn tối thiểu phải từ {voucher.MinOrderAmount:N0}đ mới xài được mã này!", 0);
        if (customerId != Guid.Empty)
        {
            var alreadyUsed = await dbContext.VoucherUsages
                .AnyAsync(u => u.CustomerId == customerId && u.VoucherId == voucher.Id && !u.IsReleased, ct);
            if (alreadyUsed) return (false, "Bạn đã dùng mã voucher này rồi, nhường người khác với! 😂", 0);
        }

        var baseForDiscount = discountBaseAmount ?? eligibilityAmount;
        return (true, "Hợp lệ!", PricingCalculator.CalculateVoucherDiscount(voucher, baseForDiscount));
    }
}