using Entities;
using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Repositories
{
    public class AgricultureProjectRepository : IDisposable
    {
        public static readonly Uri BaseUrl = new Uri("https://www.zm.gov.lv/zemkopibas-ministrija/apspriesanas/");
        public static readonly Regex PublishDateRegex = new Regex(@"\APublicēts: (\d{2}\.\d{2}\.\d{4})\z");
        private static readonly CultureInfo _cultureInfo = new CultureInfo("lv-LV");
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<AgricultureProject[]> GetAgricultureProjects()
        {
            string html = await _httpClient.GetStringAsync(BaseUrl);
            HtmlDocument htmlDocument = new HtmlDocument()
            {
                OptionEmptyCollection = true,
            };
            htmlDocument.LoadHtml(html);
            HtmlNodeCollection resultDivs = htmlDocument.DocumentNode.SelectNodes("//div[@class='content_block_item discussion_item ']");

            return resultDivs
                .Select(resultDiv =>
                {
                    HtmlNode contentNode = resultDiv.SelectSingleNode("div[@class='content_block_item_content']");
                    HtmlNode linkNode = contentNode.SelectSingleNode("div[@class='title']/a[@href]");
                    HtmlNode textNode = contentNode.SelectSingleNode("div[@class='content_block_item_content_text']/p");
                    HtmlNode dateNode = resultDiv.SelectSingleNode("div[@class='meta']/div[@class='date']/span");
                    Match dateMatch = PublishDateRegex.Match(dateNode.InnerText);

                    return new AgricultureProject
                    {
                        Name = linkNode.InnerText,
                        Description = textNode.InnerText,
                        PublishDate = DateTime.ParseExact(dateMatch.Groups[1].Value, "dd.MM.yyyy", _cultureInfo),
                        Url = new Uri(BaseUrl, linkNode.Attributes["href"].Value),
                    };
                })
                .OrderByDescending(p => p.PublishDate)
                .ToArray();
        }

        #region IDisposable Support
        bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
