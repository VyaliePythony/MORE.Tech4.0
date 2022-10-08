using MoreTech.Parser;
using MoreTech.Parser.Cache;

var dbAddress = Environment.GetEnvironmentVariable("redisdb");
if (dbAddress == null)
    dbAddress = "127.0.0.1:6379";
var parsedUrls = new RedisParserCache(dbAddress);

var parser = new Parser();

var rssInfo = File.ReadAllText("Sites.txt").Split("###");
foreach (var rss in rssInfo)
{
    Console.WriteLine($"Parsing");
    var newsInfos = await parser.ParseRSS("https://www.vedomosti.ru/rss/rubric/business/sport");
    var tasks = new List<Task<ParseResult>>();
    foreach (var newsInfo in newsInfos)
    {
        if (!await parsedUrls.ContainsAsync(newsInfo.link))
        {
            var lambda = async () =>
            {
                var text = await parser.ParseHTML(newsInfo.link, "p.box-paragraph__text");
                var result = new ParseResult()
                {
                    Url = newsInfo.link,
                    Text = text,
                    Title = newsInfo.title,
                    PublicationDate = DateTime.Parse(newsInfo.pubDate)
                };
                await parsedUrls.SetAsync(newsInfo.link, result);
                return result;
            };
            tasks.Add(lambda());
        }
    }
    await Task.WhenAll(tasks);
    Console.WriteLine($"Added {tasks.Count} news.");
}