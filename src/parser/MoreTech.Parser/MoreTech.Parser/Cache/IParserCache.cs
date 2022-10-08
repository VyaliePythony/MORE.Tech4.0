using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTech.Parser.Cache
{
    public interface IParserCache
    {
        Task<bool> ContainsAsync(string key);
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value);
    }
}
