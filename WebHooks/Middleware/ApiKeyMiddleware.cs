using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using WebHooks.Components;
using Webhooks_System_Library.Repositories;
using Webhooks_System_Library.Services;

namespace WebHooks.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IWebHookRepo repo)
        {
            //todo: complete the logic to use repo here
            //Check if request path starts with / api
            if (!httpContext.Request.Path.StartsWithSegments("/api"))
            {
                //If not API route, call await next(context) and return
                await _next.Invoke(httpContext);
                return;
            }
            if (httpContext.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next.Invoke(httpContext);
                return;
            }
            // Allow public access to create endpoints
            if (httpContext.Request.Path.StartsWithSegments("/api/endpoints")
                && httpContext.Request.Method == "POST")
            {
                await _next.Invoke(httpContext);
                return;
            }
            //Read header X-API - Key from request
            var header = httpContext.Request.Headers["X-API-KEY"].FirstOrDefault();
            //If missing, return 401 Unauthorized
            if(string.IsNullOrEmpty(header))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("401 Unauthorised : header missing API Key");
                return;
            }
            //Call GetEndpointByApiKeyAsync(apiKey)
           var result = await repo.GetEndpointsByApiKeyAsync(header);
            //If null, return 401 Unauthorized
            if(result is null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("401 unauthorized: Response is null");
                return;
            }

            //If valid, call await next(context)

            await _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
