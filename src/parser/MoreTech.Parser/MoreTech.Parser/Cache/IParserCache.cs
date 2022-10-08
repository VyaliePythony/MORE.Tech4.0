using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTech.Parser.Cache
{
    public interface IParserCache<TKey, TValue>
    {
        Task<bool> ContainsAsync(TKey key);
        Task<TValue?> GetAsync(TKey key);
        Task SetAsync(TKey key, TValue value);
    }
}
