using Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace FeedGenerator.Controllers
{
    [Route("[controller]")]
    public class RigaBuildingPermitsController : Controller
    {
        private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        private readonly RigaBuildingPermitRepository _repository = new RigaBuildingPermitRepository();

        public async Task<ActionResult> Get(string search = null)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _timeZone);
            DateTime dateFrom = now.AddMonths(-1);

            RigaBuildingPermit[] buildingPermits = await _repository.GetBuildingPermits(dateFrom, search);

            string title = "Rīgas būvatļaujas";

            if (!string.IsNullOrEmpty(search))
            {
                title += $" - {search}";
            }

            SyndicationFeed feed = new SyndicationFeed(title, null, new Uri(RigaBuildingPermitRepository.BaseUri))
            {
                Items = buildingPermits
                    .Select(buildingPermit => CreateSyndicationItem(buildingPermit))
                    .ToArray(),
            };

            return new FeedActionResult(feed);
        }

        private SyndicationItem CreateSyndicationItem(RigaBuildingPermit buildingPermit)
        {
            TimeSpan utcOffset = _timeZone.GetUtcOffset(buildingPermit.PreparationDate);

            string searchString = buildingPermit.Object
                .Replace('(', ' ')
                .Replace(')', ' ');

            while (searchString.Contains("  "))
            {
                searchString = searchString.Replace("  ", " ");
            }

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                ["search"] = searchString
            };
            query["date_to"] = query["date_from"] = buildingPermit.PreparationDate.ToString(RigaBuildingPermitRepository.DateFormat);

            UriBuilder uriBuilder = new UriBuilder(RigaBuildingPermitRepository.BaseUri)
            {
                Query = Helper.GetQueryString(query),
            };

            SyndicationItem item = new SyndicationItem(buildingPermit.Object, buildingPermit.ObjectAddress, uriBuilder.Uri)
            {
                PublishDate = new DateTimeOffset(buildingPermit.PreparationDate, utcOffset),
            };

            return item;
        }

        #region IDisposable Support
        bool _disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _repository.Dispose();
                }

                _disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
