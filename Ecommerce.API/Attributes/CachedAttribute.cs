using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.API;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CachedAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLiveSeconds = timeToLiveSeconds;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ICacheService? cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        string? cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
        string? cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            ContentResult? contentResult = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            context.Result = contentResult;
            return;
        }

        ActionExecutedContext? executedContext = await next(); 

        if (executedContext.Result is OkObjectResult okObjectResult)
        {
            await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value!, TimeSpan.FromSeconds(_timeToLiveSeconds));
        }
    }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        StringBuilder? keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}
