using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using PasswordManagerApp.Models;

namespace PasswordManagerApp.Cache
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> GetOrCreateCachedResponse<T>(string cacheKey, Func<Task<IEnumerable<T>>> func) where T : class;
        IEnumerable<T> GetOrCreateCachedResponseSync<T>(string cacheKey, Func<IEnumerable<T>> func) where T : class;
        Task<T> GetOrCreateCachedResponse<T>(string cacheKey, Func<Task<T>> func) where T : class;
        void ClearCache(string cacheKey);
    }

    public class CacheService : ICacheService
    {

        
        private readonly ICacheProvider _cacheProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        

        public CacheService(ICacheProvider cacheProvider,IHttpContextAccessor httpContextAccessor)
        {
            
            _cacheProvider = cacheProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        private string AuthUserId { get { return  _httpContextAccessor.HttpContext.User.Identity.Name; } }

        public void ClearCache(string cacheKey)
        {
            _cacheProvider.ClearCache(cacheKey);
            _cacheProvider.ClearCache(CacheKeys.Statistics+AuthUserId);
        }

        public async Task<IEnumerable<T>> GetOrCreateCachedResponse<T>(string cacheKey, Func<Task<IEnumerable<T>>> func) where T : class
        {
            var data = _cacheProvider.GetFromCache<IEnumerable<T>>(cacheKey);
            if (data != null) 
                return data;

            // Key not in cache, so get data.
            data = await func();

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            _cacheProvider.SetCache(cacheKey, data, cacheEntryOptions);

            return data;
        }
        public  IEnumerable<T> GetOrCreateCachedResponseSync<T>(string cacheKey, Func<IEnumerable<T>> func) where T:class
        {
            var data = _cacheProvider.GetFromCache<IEnumerable<T>>(cacheKey);
            if (data != null)
                return data;

            // Key not in cache, so get data.
            data =  func();

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            _cacheProvider.SetCache(cacheKey, data, cacheEntryOptions);

            return data;
        }

        public async Task<T> GetOrCreateCachedResponse<T>(string cacheKey, Func<Task<T>> func) where T : class
        {
            var data = _cacheProvider.GetFromCache<T>(cacheKey);
            if (data != null) 
                return data;

            // Key not in cache, so get data.
            data = await func();

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            _cacheProvider.SetCache(cacheKey, data, cacheEntryOptions);

            return data;
        }

        


    }
}
