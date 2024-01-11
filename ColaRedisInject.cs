using Cola.ColaCache.ColaCache;
using Cola.ColaCache.IColaCache;
using Cola.Core.ColaConsole;
using Cola.Core.Models.ColaCache;
using Cola.Core.Utils.Constants;
using Cola.CoreUtils.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.ColaCache;

public static class ColaRedisInject
{
    public static IServiceCollection AddColaCache(
        this IServiceCollection services,
        IConfiguration config)
    {
        var cacheConfig = config.GetColaSection<CacheConfigOption>(SystemConstant.CONSTANT_COLACACHE_SECTION);
        cacheConfig ??= new CacheConfigOption();
        return InjectCache(services, cacheConfig);
    }
    
    
    public static IServiceCollection AddColaCache(
        this IServiceCollection services,
        Action<CacheConfigOption> action)
    {
        var cacheConfig = new CacheConfigOption();
        action(cacheConfig);
        return InjectCache(services, cacheConfig);
    }

    private static IServiceCollection InjectCache(IServiceCollection services, CacheConfigOption cacheConfig)
    {
        if (cacheConfig.CacheType == CacheType.NoCache.ToInt())
        {
            return services;
        }
        if (cacheConfig.CacheType == CacheType.Hybrid.ToInt())
        {
            services.AddSingleton<IColaRedisCache>(provider => new ColaRedis(cacheConfig));
            ConsoleHelper.WriteInfo("ColaCache 注入类型【 IColaRedisCache 】");
            services.AddSingleton<IColaMemoryCache>(provider => new ColaMemoryCache(cacheConfig));
            ConsoleHelper.WriteInfo("ColaCache 注入类型【 IColaMemoryCache 】");
            services.AddSingleton<IColaHybridCache,ColaHybridCache>();
            services.AddSingleton<IColaHybridCache>(servicesProvider=>ColaHybridCache.Create(servicesProvider));
            ConsoleHelper.WriteInfo("ColaCache 注入类型【 IColaHybridCache 】");
        }
        else if (cacheConfig.CacheType == CacheType.Redis.ToInt())
        {
            services.AddSingleton<IColaRedisCache>(provider => new ColaRedis(cacheConfig));
            ConsoleHelper.WriteInfo("ColaCache 注入类型【 IColaRedisCache 】");
        }
        else if (cacheConfig.CacheType == CacheType.InMemory.ToInt())
        {
            services.AddSingleton<IColaMemoryCache>(provider => new ColaMemoryCache(cacheConfig));
            ConsoleHelper.WriteInfo("ColaCache 注入类型【 IColaMemoryCache 】");
        }
        return services;
    }
}