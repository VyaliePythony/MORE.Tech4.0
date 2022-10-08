using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTech.Parser.Cache
{
    public class DictionaryParserCache<TKey, TValue> : IParserCache<TKey, TValue>
    {
        public readonly ConcurrentDictionary<TKey, TValue> Cache = new ConcurrentDictionary<TKey, TValue>();

        public async Task<bool> ContainsAsync(TKey key)
        {
            return Cache.ContainsKey(key);
        }

        public async Task<TValue?> GetAsync(TKey key)
        {
            Cache.TryGetValue(key, out var value);
            return value;
        }

        public async Task SetAsync(TKey key, TValue value)
        {
            Cache[key] = value;
        }
    }
}
