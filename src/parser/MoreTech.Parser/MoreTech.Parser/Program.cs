using MoreTech.Parser;
using MoreTech.Parser.Cache;
using System.Collections.Concurrent;
using System.Reflection;

var dbAddress = Environment.GetEnvironmentVariable("REDIS-DB");
if (dbAddress == null)
    dbAddress = "127.0.0.1:6379";
var parsedUrls = new RedisParserCache(dbAddress);
var pathToFile = "//appdata//data.csv";
var lines = new ConcurrentBag<ParseResult>();

while (true)
{
    // берем разные источники новостей из RSS каналов
    var rssInfo = Sites.All.Split("###");
    foreach (var rss in rssInfo)
    {
        var splitResult = rss.Trim().Split('\n');
        var queryString = splitResult[0];  // к каждому сайту задается селектор для блоков с текстом
        var tasks = new List<Task>();
        for (int i = 1; i < splitResult.Length; i++)
        {
            var url = splitResult[i];  // путь к RSS-каналу
            if (String.IsNullOrEmpty(url))
                continue;
            Console.WriteLine($"Parsing {url} RSS");
            var newsInfos = await Parser.ParseRSS(url);  // получение всех <item>, т.е. получение информации о каждой новости
            if (newsInfos == null)
                continue;
            foreach (var newsInfo in newsInfos)
            {
                // если в БД уже есть данная новость, то ее не надо парсить
                if (!await parsedUrls.ContainsAsync(newsInfo.link))
                {
                    //tasks.Add(Utils.ParseAndSave(newsInfo, queryString, parsedUrls, lines));
                    var task = new Task(() => Utils.ParseAndSave(newsInfo, queryString, parsedUrls, lines));
                    tasks.Add(task);
                }
                else
                {
                    // уже отпарсенная новость достается из БД
                    var existingNews = await parsedUrls.GetAsync(newsInfo.link);
                    if (existingNews != null)
                        lines.Add(existingNews);
                }
            }
            //защита от спама запросами
            var buffer = new List<Task>();
            foreach (var task in tasks)
            {
                if (buffer.Count < 40)
                {
                    task.Start();
                    buffer.Add(task);
                    continue;
                }
                await Task.WhenAll(buffer);
                Console.WriteLine("Resting for 10 sec");
                Thread.Sleep(10000);
                task.Start();
                buffer = new List<Task>() { task };
            }
            await Task.WhenAll(tasks);
        }
    }
    Thread.Sleep(2000);
    // создание .csv файла с новостями
    File.WriteAllText(pathToFile, "");
    foreach (var line in lines)
    {
        var newString = $"\"{line.Url}\";\"{line.Title.Replace(';', ',')}\";\"{line.Text.Replace(';', ',')}\";\"{line.PublicationDate}\"\n";
        File.AppendAllText(pathToFile, newString);
    }
    Console.WriteLine($".csv file created at {pathToFile}");
    Thread.Sleep(30 * 60 * 1000);
}


public static class Utils
{
    public static async Task ParseAndSave(
        (string link, string title, string pubDate) newsInfo,
        string queryString,
        IParserCache<string, ParseResult> parsedUrls,
        ConcurrentBag<ParseResult> lines)
    {
        // парсинг страницы с новостью
        var text = await Parser.ParseHTML(newsInfo.link, queryString);
        var result = new ParseResult()
        {
            Url = newsInfo.link,
            Text = text,
            Title = newsInfo.title,
            PublicationDate = DateTime.Parse(newsInfo.pubDate)
        };
        if (result != null)
        {
            // добавление в БД отпарсенной новости
            await parsedUrls.SetAsync(newsInfo.link, result);
            lines.Add(result);
        }
    }
}