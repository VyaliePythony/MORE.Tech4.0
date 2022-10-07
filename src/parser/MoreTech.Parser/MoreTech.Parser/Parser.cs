﻿using AngleSharp.Html.Dom;
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
            var htmlClient = new HttpClient();
            var htmlResponse = await htmlClient.GetAsync(url);

            if (htmlResponse != null && htmlResponse.StatusCode == HttpStatusCode.OK)
            {
                var rawHtml = await htmlResponse.Content.ReadAsStringAsync();
                var htmlParser = new HtmlParser();
                var html = await htmlParser.ParseDocumentAsync(rawHtml);
                var paragraphs = html.QuerySelectorAll(querySelector)
                    .Select(p => p.TextContent);
                var text = string.Concat(paragraphs);
                return text;
            }
            return null;
        }

        public async IAsyncEnumerable<(string link, string title, string pubDate)> ParseRSS(string rssUrl)
        {
            var xmlClient = new HttpClient();
            var xmlResponse = await xmlClient.GetAsync(rssUrl);

            if (xmlResponse != null && xmlResponse.StatusCode == HttpStatusCode.OK)
            {
                var rawXml = await xmlResponse.Content.ReadAsStringAsync();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rawXml);
                var xmlRoot = xmlDoc.DocumentElement;
                var node = xmlRoot!.FirstChild;
                foreach (XmlNode n in node.SelectNodes("item"))
                {
                    var link = n.SelectSingleNode("link").InnerText;
                    var title = n.SelectSingleNode("title").InnerText;
                    var pubdate = n.SelectSingleNode("pubDate").InnerText;
                    yield return (link, title, pubdate);
                }
            }
            yield break;
        }

        public ParseResult ParseRaw(IHtmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
