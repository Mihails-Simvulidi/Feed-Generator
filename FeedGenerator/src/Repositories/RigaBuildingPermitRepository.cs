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

        static CultureInfo _cultureInfo = new CultureInfo("lv-LV");

        HttpClient _httpClient = new HttpClient();

        public async Task<RigaBuildingPermit[]> GetBuildingPermits(string searchString = null)
        {
            var dateFrom = DateTime.UtcNow.AddMonths(-1);

            var query = new Dictionary<string, string>();
            query["date_from"] = dateFrom.ToString(DateFormat);

            if (!string.IsNullOrEmpty(searchString))
                query["search"] = searchString;

            var uriBuilder = new UriBuilder(BaseUri)
            {
                Query = Helper.GetQueryString(query),
            };

            var html = await _httpClient.GetStringAsync(uriBuilder.Uri);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var resultTable = htmlDocument.GetElementbyId("results");

            return resultTable
                .Elements("tr")
                .Skip(1)
                .Select(row =>
                {
                    var columns = row.Elements("td").ToArray();
                    return new RigaBuildingPermit
                    {
                        PreparationDate = DateTime.ParseExact(columns[0].InnerText, DateFormat, _cultureInfo),
                        Object = columns[4].InnerText,
                        ObjectAddress = columns[5].InnerText,
                    };
                })
                .ToArray();
        }

        #region IDisposable Support
        bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _httpClient.Dispose();

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
