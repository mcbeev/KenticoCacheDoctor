using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;

namespace KenticoCacheDoctor
{
    public static class CacheItemsBuilder
    {
        public static IOrderedEnumerable<KeyValuePair<string, object>> GetAllCacheItems(Cache cache)
        {
            var cacheItems = new Dictionary<string, object>();

            var enumerator = cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var key = (string)enumerator.Key;

                if (key.StartsWith(Constants.KenticoOutputCacheKeyPageIdentifier, StringComparison.OrdinalIgnoreCase) ||
                    (key.StartsWith(Constants.KenticoOutputCacheKeyNodesAllIdentifierStart, StringComparison.OrdinalIgnoreCase) &&
                    key.EndsWith(Constants.KenticoOutputCacheKeyNodesAllIdentifierEnd, StringComparison.OrdinalIgnoreCase)))
                {
                    //Sort the most important ones to the top, the ones you would want to Bust
                    key = Constants.CacheKeyMarker + key;
                }

                cacheItems.Add(key, enumerator.Value);
            }

            return cacheItems.OrderBy(i => i.Key);
        }
    }
}
