namespace Pricing.Grpc.Interceptors;

public class GrpcExceptionInterceptor 
    (ILogger<GrpcExceptionInterceptor> logger)
    : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>
        (TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await continuation(request, context);
            sw.Stop();
            logger.LogInformation("gRPC {Method} xong trong {ElapsedMs}ms ✅", context.Method, sw.ElapsedMilliseconds);
            return response;
        } catch (RpcException)
        {
            throw;
        } catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Tham số gRPC {Method} tào lao rồi sếp ơi", context.Method);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        } catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Đụng độ concurrency khi giật voucher tại {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Aborted, "Voucher đang bị tranh giành, thử lại nha!"));
        } catch (Exception ex)
        {
            logger.LogError(ex, "Toang rồi ông giáo ơi tại gRPC {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Lỗi hệ thống nội bộ tại Pricing.Grpc"));
        }
    }
}