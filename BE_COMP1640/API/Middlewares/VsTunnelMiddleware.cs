namespace API.Middlewares
{
    public class VsTunnelMiddleware
    {
        private readonly RequestDelegate _next;

        public VsTunnelMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            // Kiểm tra xem biến môi trường VS_TUNNEL_URL có tồn tại không
            var vsTunnelUrl = Environment.GetEnvironmentVariable("VS_TUNNEL_URL");
            if (!string.IsNullOrEmpty(vsTunnelUrl))
            {
                // Chuyển đổi VS_TUNNEL_URL thành HostString
                var uri = new Uri(vsTunnelUrl);
                var hostString = new HostString(uri.Host, uri.Port);

                // Cập nhật context.Request.Host
                context.Request.Host = hostString;
            }

            // Chuyển tiếp yêu cầu đến middleware tiếp theo trong pipeline
            return _next(context);
        }
    }
}
