using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTech.Parser.Cache
{
    public class HashsetParserCache : IParserCache
    {
        private HashSet<string> cache = new HashSet<string>();

        public async Task<bool> ContainsAsync(string key)
        {
            return cache.Contains(key);
        }

        public async Task<string?> GetAsync(string key)
        {
            cache.TryGetValue(key, out var value);
            return value;
        }

        public async Task SetAsync(string key, string value)
        {
            cache.Add(value);
        }
    }
}
