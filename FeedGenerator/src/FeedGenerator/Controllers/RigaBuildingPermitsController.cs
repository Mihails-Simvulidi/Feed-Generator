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
        static TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        RigaBuildingPermitRepository _repository = new RigaBuildingPermitRepository();

        public async Task<ActionResult> Get(string search = null)
        {
            var utcNow = DateTime.UtcNow;
            var now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _timeZone);
            var dateFrom = now.AddMonths(-1);

            var buildingPermits = await _repository.GetBuildingPermits(dateFrom, search);

            var title = "Rīgas būvatļaujas";

            if (!string.IsNullOrEmpty(search))
                title += $" - {search}";

            var feed = new SyndicationFeed(title, null, new Uri(RigaBuildingPermitRepository.BaseUri))
            {
                Items = buildingPermits
                    .Select(buildingPermit => CreateSyndicationItem(buildingPermit))
                    .ToArray(),
            };

            return new FeedActionResult(feed);
        }

        SyndicationItem CreateSyndicationItem(RigaBuildingPermit buildingPermit)
        {
            var utcOffset = _timeZone.GetUtcOffset(buildingPermit.PreparationDate);

            var searchString = buildingPermit.Object
                .Replace('(', ' ')
                .Replace(')', ' ');

            while (searchString.Contains("  "))
                searchString = searchString.Replace("  ", " ");

            var query = new Dictionary<string, string>();
            query["search"] = searchString;
            query["date_to"] = query["date_from"] = buildingPermit.PreparationDate.ToString(RigaBuildingPermitRepository.DateFormat);

            var uriBuilder = new UriBuilder(RigaBuildingPermitRepository.BaseUri)
            {
                Query = Helper.GetQueryString(query),
            };

            var item = new SyndicationItem(buildingPermit.Object, buildingPermit.ObjectAddress, uriBuilder.Uri)
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
                    _repository.Dispose();

                _disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
