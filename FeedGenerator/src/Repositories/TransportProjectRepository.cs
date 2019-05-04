using Entities;
using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace Repositories
{
    public class TransportProjectRepository : IDisposable
    {
        public static readonly Uri BaseUrl = new Uri("http://www.sam.gov.lv/satmin/content/?cat=553");
        private static readonly CultureInfo _cultureInfo = new CultureInfo("lv-LV");
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<TransportProject[]> GetTransportProjects()
        {
            string html = await _httpClient.GetStringAsync(BaseUrl);
            HtmlDocument htmlDocument = new HtmlDocument()
            {
                OptionAutoCloseOnEnd = true,
                OptionEmptyCollection = true,
            };
            htmlDocument.LoadHtml(html);
            HtmlNode resultTableBody = htmlDocument.DocumentNode.SelectSingleNode("//table[@class='rte_table']/tbody");

            return resultTableBody
                .Elements("tr")
                .Skip(1)
                .Select(row =>
                {
                    HtmlNode[] columns = row.Elements("td").ToArray();

                    if (columns.Length < 4)
                    {
                        return null;
                    }

                    return new TransportProject
                    {
                        Name = columns[0].InnerText,
                        PublishDate = DateTime.ParseExact(columns[1].InnerText, "dd.MM.yyyy", _cultureInfo),
                        ApplyingInfo = columns[3].InnerText,
                    };
                })
                .Where(project => project != null)
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
