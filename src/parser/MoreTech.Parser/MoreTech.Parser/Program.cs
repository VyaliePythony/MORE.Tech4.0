using MoreTech.Parser;
using MoreTech.Parser.Cache;
using System.Reflection;

var dbAddress = Environment.GetEnvironmentVariable("REDIS-DB");
if (dbAddress == null)
    dbAddress = "127.0.0.1:6379";
var parsedUrls = new RedisParserCache(dbAddress);
var pathToFile = "//appdata/data.csv";

var parser = new Parser();
object _lock = new object();

while (true)
{
    var rssInfo = Sites.All.Split("###");
    foreach (var rss in rssInfo)
    {
        var splitResult = rss.Trim().Split('\n');
        var queryString = splitResult[0];
        var tasks = new List<Task<ParseResult>>();
        for (int i = 1; i < splitResult.Length; i++)
        {
            var url = splitResult[i];
            if (String.IsNullOrEmpty(url))
                continue;
            Console.WriteLine($"Parsing {url} RSS");
            var newsInfos = await parser.ParseRSS(url);
            foreach (var newsInfo in newsInfos)
            {
                if (!await parsedUrls.ContainsAsync(newsInfo.link))
                {
                    var lambda = async () =>
                    {
                        var text = await parser.ParseHTML(newsInfo.link, queryString);
                        var result = new ParseResult()
                        {
                            Url = newsInfo.link,
                            Text = text,
                            Title = newsInfo.title,
                            PublicationDate = DateTime.Parse(newsInfo.pubDate)
                        };
                        await parsedUrls.SetAsync(newsInfo.link, result);
                        var newString = $"\"{result.Url}\";\"{result.Title}\";\"{result.Text}\";\"{result.PublicationDate}\"\n";
                        lock (_lock)
                        {
                            File.AppendAllText(pathToFile, newString);
                        }
                        return result;
                    };
                    tasks.Add(lambda());
                }
                else
                {
                    var existingNews = await parsedUrls.GetAsync(newsInfo.link);
                    var newString = $"\"{existingNews.Url}\";\"{existingNews.Title}\";\"{existingNews.Text}\";\"{existingNews.PublicationDate}\"\n";
                    lock (_lock)
                    {
                        File.AppendAllText(pathToFile, newString);
                    }
                }
            }
            await Task.WhenAll(tasks);
        }
    }
    Thread.Sleep(30 * 60 * 60);
}
