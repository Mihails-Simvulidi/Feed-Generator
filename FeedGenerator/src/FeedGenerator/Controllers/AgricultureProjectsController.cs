using Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace FeedGenerator.Controllers
{
    [Route("[controller]")]
    public class AgricultureProjectsController : Controller
    {
        private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        private readonly AgricultureProjectRepository _repository = new AgricultureProjectRepository();

        public async Task<ActionResult> Get()
        {
            AgricultureProject[] agricultureProjects = await _repository.GetAgricultureProjects();

            SyndicationFeed feed = new SyndicationFeed("Zemkopības ministrijas sabiedriskās apspriešanas", null, AgricultureProjectRepository.BaseUrl)
            {
                Items = agricultureProjects
                    .Select(agricultureProject => CreateSyndicationItem(agricultureProject))
                    .ToArray(),
            };

            return new FeedActionResult(feed);
        }

        private SyndicationItem CreateSyndicationItem(AgricultureProject agricultureProject)
        {
            TimeSpan utcOffset = _timeZone.GetUtcOffset(agricultureProject.PublishDate);

            SyndicationItem item = new SyndicationItem(agricultureProject.Name, agricultureProject.Description, agricultureProject.Url)
            {
                PublishDate = new DateTimeOffset(agricultureProject.PublishDate, utcOffset),
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
