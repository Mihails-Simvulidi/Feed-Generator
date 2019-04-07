using Entities;
using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace Repositories
{
    public class RigaBuildingPermitRepository : IDisposable
    {
        public const string BaseUri = "http://atdep.rcc.lv/exp/buve/atlaujas.aspx";
        public const string DateFormat = "dd.MM.yyyy";
        private static readonly CultureInfo _cultureInfo = new CultureInfo("lv-LV");
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<RigaBuildingPermit[]> GetBuildingPermits(DateTime dateFrom, string searchString = null)
        {
            Dictionary<string, string> query = new Dictionary<string, string>
            {
                ["date_from"] = dateFrom.ToString(DateFormat)
            };

            if (!string.IsNullOrEmpty(searchString))
            {
                query["search"] = searchString;
            }

            UriBuilder uriBuilder = new UriBuilder(BaseUri)
            {
                Query = Helper.GetQueryString(query),
            };

            string html = await _httpClient.GetStringAsync(uriBuilder.Uri);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNode resultTable = htmlDocument.GetElementbyId("results");

            return resultTable
                .Elements("tr")
                .Skip(1)
                .Select(row =>
                {
                    HtmlNode[] columns = row.Elements("td").ToArray();
                    return new RigaBuildingPermit
                    {
                        PreparationDate = DateTime.ParseExact(columns[0].InnerText, DateFormat, _cultureInfo),
                        Object = columns[4].InnerText,
                        ObjectAddress = columns[5].InnerText,
                    };
                })
                .OrderByDescending(p => p.PreparationDate)
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
