using System;
using System.Collections;
using System.Runtime.Caching;
using System.Web.Caching;

namespace KenticoCacheDoctor
{
    public static class CacheExtensions
    {
        //Returns approximate size in bytes of the Cache
        //Idea taken from https://stackoverflow.com/questions/22392634/how-to-measure-current-size-of-net-memory-cache-4-0/45078655
        // and https://stackoverflow.com/questions/605621/how-to-get-object-size-in-memory
        //This is not 100% accurate, but close enough to see relative size.
        public static long GetApproximateSize(this Cache cache)
        {
            long cacheApproximateSize = 0;
            long cacheSizeBaseline = GC.GetTotalMemory(false);

            using (MemoryCache memCache = new MemoryCache("temp"))
            {
                IDictionaryEnumerator enumerator = cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    memCache.Add((string)enumerator.Key, enumerator.Value, new CacheItemPolicy());
                }

                cacheApproximateSize = GC.GetTotalMemory(false);

                memCache.Dispose();
            }

            return (cacheApproximateSize - cacheSizeBaseline);
        }
    }
}
