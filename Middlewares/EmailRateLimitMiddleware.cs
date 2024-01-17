using System;
using System.Collections.Concurrent;
using System.Net;

namespace BiblioServer.Middlewares
{
    public class EmailRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new ConcurrentDictionary<string, DateTime>();
        private readonly int _waitTime;
        private readonly string _serviceIdentifier; 

        public EmailRateLimitMiddleware(RequestDelegate next, int waitTime, string serviceIdentifier)
        {
            _next = next;
            _waitTime = waitTime;
            _serviceIdentifier = serviceIdentifier;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_serviceIdentifier != null)
            {
                string serviceKey = $"{_serviceIdentifier}:{context.Connection.RemoteIpAddress?.ToString()?.ToLower()}";

                if (!string.IsNullOrEmpty(serviceKey) && _lastRequestTimes.TryGetValue(serviceKey, out DateTime lastRequestTime))
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

                _lastRequestTimes.AddOrUpdate(serviceKey, DateTime.Now, (key, value) => DateTime.Now);
            }

            await _next(context);
        }
    }

}

