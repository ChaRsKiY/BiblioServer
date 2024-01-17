using System.Collections.Concurrent;
using System.Net;

public class RequestRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new ConcurrentDictionary<string, DateTime>();
    private readonly int _waitTime;
    private readonly string _routeName;

    public RequestRateLimitMiddleware(RequestDelegate next, int waitTime, string routeName)
    {
        _next = next;
        _waitTime = waitTime;
        _routeName = routeName;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(_routeName))
        {
            string ipAddress = context.Connection.RemoteIpAddress?.ToString()?.ToLower();

            if (!string.IsNullOrEmpty(ipAddress) && _lastRequestTimes.TryGetValue(ipAddress, out DateTime lastRequestTime))
            {
                var timeSinceLastRequest = DateTime.Now - lastRequestTime;
                var timeToWait = TimeSpan.FromMinutes(_waitTime) - timeSinceLastRequest;

                if (timeToWait > TimeSpan.Zero)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.Headers.Add("Retry-After", timeToWait.TotalSeconds.ToString());
                    return;
                }
            }

            _lastRequestTimes.AddOrUpdate(ipAddress, DateTime.Now, (key, value) => DateTime.Now);
        }

        await _next(context);
    }
}
