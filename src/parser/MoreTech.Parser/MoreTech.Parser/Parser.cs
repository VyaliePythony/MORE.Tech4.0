using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MoreTech.Parser
{
    public class ParseResult
    { 
        public DateTime PublicationDate { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }


    public class Parser
    {
        public async Task<string> ParseHTML(string url, string querySelector)
        {
            using (var htmlClient = new HttpClient())
            {
                try
                {
                    var htmlResponse = await htmlClient.GetAsync(url).ConfigureAwait(false);

                    if (htmlResponse != null && htmlResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var rawHtml = await htmlResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var htmlParser = new HtmlParser();
                        var html = await htmlParser.ParseDocumentAsync(rawHtml).ConfigureAwait(false);
                        var paragraphs = html.QuerySelectorAll(querySelector)
                            .Select(p => p.TextContent);
                        var text = string.Concat(paragraphs);
                        return text;
                    }
                    return null;
                }
                catch (Exception)
                {
                    Console.WriteLine($"{url} failed.");
                }
            }
            return null;
        }

        public async Task<IEnumerable<(string link, string title, string pubDate)>> ParseRSS(string rssUrl)
        {
            using (var xmlClient = new HttpClient())
            {
                xmlClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
                try
                {
                    var xmlResponse = await xmlClient.GetAsync(rssUrl);

                    if (xmlResponse != null && xmlResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var rawXml = await xmlResponse.Content.ReadAsStringAsync();
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(rawXml);
                        var xmlRoot = xmlDoc.DocumentElement;
                        var node = xmlRoot!.FirstChild;
                        var list = new List<(string link, string title, string pubDate)>();
                        foreach (XmlNode n in node.SelectNodes("item"))
                        {
                            var link = n.SelectSingleNode("link").InnerText;
                            var title = n.SelectSingleNode("title").InnerText;
                            var pubdate = n.SelectSingleNode("pubDate").InnerText;
                            list.Add((link, title, pubdate));
                        }
                        return list;
                    }
                    return null;
                }
                catch (Exception)
                {
                    Console.WriteLine($"{rssUrl} failed.");
                }
            }
            return null;
        }

        public ParseResult ParseRaw(IHtmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
