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
    public class TransportProjectsController : Controller
    {
        private readonly TransportProjectRepository _repository = new TransportProjectRepository();

        public async Task<ActionResult> Get()
        {
            SyndicationFeed feed = new SyndicationFeed("Satiksmes ministrijas izstrādē esošie attīstības plānošanas dokumenti un tiesību akti", null, TransportProjectRepository.BaseUrl);

            try
            {
                TransportProject[] transportProjects = await _repository.GetTransportProjects();

                feed.Items = transportProjects
                    .Select(transportProject => CreateSyndicationItem(transportProject))
                    .ToArray();
            }
            catch (Exception e)
            {
                FeedHelper.AddExceptionToFeed(feed, e);
            }

            return new FeedActionResult(feed);
        }

        private SyndicationItem CreateSyndicationItem(TransportProject transportProject)
        {
            SyndicationItem item = new SyndicationItem(transportProject.Name, transportProject.ApplyingInfo, TransportProjectRepository.BaseUrl)
            {
                PublishDate = FeedHelper.GetDateTimeOffsetLV(transportProject.PublishDate),
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
