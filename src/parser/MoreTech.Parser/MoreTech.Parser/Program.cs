using MoreTech.Parser;
using MoreTech.Parser.Cache;

var parser = new Parser();
var newsInfos = await parser.ParseRSS("https://www.vedomosti.ru/rss/rubric/business/sport");
var tasks = new List<Task<string>>();
IParserCache parsedUrls = new HashsetParserCache();

foreach (var newsInfo in newsInfos.Take(30))
{
    tasks.Add(parser.ParseHTML(newsInfo.link, "p.box-paragraph__text"));
    await parsedUrls.SetAsync(null, newsInfo.link);
}
await Task.WhenAll(tasks);
foreach (var task in tasks)
{
    Console.WriteLine(String.Join("", task.Result.Take(30)));
}

newsInfos = await parser.ParseRSS("https://www.vedomosti.ru/rss/rubric/business/sport");
tasks = new List<Task<string>>();
foreach (var newsInfo in newsInfos.Take(30))
{
    if (!await parsedUrls.ContainsAsync(newsInfo.link))
        tasks.Add(parser.ParseHTML(newsInfo.link, "p.box-paragraph__text"));
}
await Task.WhenAll(tasks);
foreach (var task in tasks)
{
    Console.WriteLine(String.Join("", task.Result.Take(30)));
}