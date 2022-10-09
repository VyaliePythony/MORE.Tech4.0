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


    public static class Parser
    {
        /// <summary>
        /// Парсит указанную ссылку как html
        /// </summary>
        /// <param name="url">Ссылка на страницу</param>
        /// <param name="querySelector">Селектор нужной части страницы</param>
        /// <returns>Строку, т.е. внутренний текст выделенных с помощью querySelector тегов</returns>
        public static async Task<string> ParseHTML(string url, string querySelector)
        {
            using (var htmlClient = new HttpClient())
            {
                htmlClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
                try
                {
                    var htmlResponse = await htmlClient.GetAsync(url).ConfigureAwait(false);
                    Console.WriteLine($"{url} responsed with {htmlResponse.StatusCode}.");
                    if (htmlResponse != null && htmlResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var rawHtml = await htmlResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var htmlParser = new HtmlParser();
                        var html = await htmlParser.ParseDocumentAsync(rawHtml).ConfigureAwait(false);
                        var paragraphs = html.QuerySelectorAll(querySelector)
                            .Select(p => p.TextContent);
                        var text = string.Concat(paragraphs).Replace("\n", "");
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


        /// <summary>
        /// Парсит RSS канал, т.к. RSS имеет стандартную структуру, то достаточно просто взять root элемент, взять первого ребенка <rss>, затем найти все внутренние <item>, т.е. информацию с новостями
        /// </summary>
        /// <param name="rssUrl">Ссылка на RSS канал</param>
        /// <returns>Кортеж из ссылки на полную новость, ее заголовка и даты публикации</returns>
        public static async Task<IEnumerable<(string link, string title, string pubDate)>> ParseRSS(string rssUrl)
        {
            using (var xmlClient = new HttpClient())
            {
                try
                {
                    var xmlResponse = await xmlClient.GetAsync(rssUrl);
                    
                    if (xmlResponse != null && xmlResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var rawXml = await xmlResponse.Content.ReadAsByteArrayAsync();
                        var xml = System.Text.Encoding.UTF8.GetString(rawXml);
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);
                        var xmlRoot = xmlDoc.DocumentElement;
                        var node = xmlRoot!.FirstChild;
                        var list = new List<(string link, string title, string pubDate)>();
                        foreach (XmlNode n in node.SelectNodes("item"))
                        {
                            var link = n.SelectSingleNode("link").InnerText;
                            var title = RemoveCDATA(n.SelectSingleNode("title").InnerText).Replace("\n", "");
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

        public static ParseResult ParseRaw(IHtmlDocument document)
        {
            throw new NotImplementedException();
        }

        private static string RemoveCDATA(string str)
        {
            string result = "";
            str = str.Replace("CDATA", "");
            str = str.Replace("!", "");
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            return str;
        }
    }
}
