using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTech.Parser.Cache
{
    public class RedisParserCache : IParserCache<string, ParseResult>
    {
        private readonly IDatabase _db;

        public RedisParserCache(string dbAddress)
        {
            var redis = ConnectionMultiplexer.Connect($"{dbAddress},abortConnect=false");
            _db = redis.GetDatabase();
        }

        public async Task<bool> ContainsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<ParseResult?> GetAsync(string key)
        {
            var url = await _db.HashGetAsync(key, "Url");
            var date = await _db.HashGetAsync(key, "PublicationDate");
            var title = await _db.HashGetAsync(key, "Title");
            var text = await _db.HashGetAsync(key, "Text");
            return new ParseResult()
            {
                Url = url,
                PublicationDate = DateTime.Parse(date),
                Title = title,
                Text = text
            };
        }

        public async Task SetAsync(string key, ParseResult value)
        {
            var hash = new HashEntry[] {
                new HashEntry("Url", value.Url),
                new HashEntry("PublicationDate", value.PublicationDate.ToString()),
                new HashEntry("Title", value.Title),
                new HashEntry("Text", value.Text)
            };
            await _db.HashSetAsync(key, hash);
        }
    }
}
