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
        private readonly AgricultureProjectRepository _repository = new AgricultureProjectRepository();

        public async Task<ActionResult> Get()
        {
            SyndicationFeed feed = new SyndicationFeed("Zemkopības ministrijas sabiedriskās apspriešanas", null, AgricultureProjectRepository.BaseUrl);

            try
            {
                AgricultureProject[] agricultureProjects = await _repository.GetAgricultureProjects();

                feed.Items = agricultureProjects
                    .Select(agricultureProject => CreateSyndicationItem(agricultureProject))
                    .ToArray();
            }
            catch (Exception e)
            {
                FeedHelper.AddExceptionToFeed(feed, e);
            }

            return new FeedActionResult(feed);
        }

        private SyndicationItem CreateSyndicationItem(AgricultureProject agricultureProject)
        {
            SyndicationItem item = new SyndicationItem(agricultureProject.Name, agricultureProject.Description, agricultureProject.Url)
            {
                PublishDate = FeedHelper.GetDateTimeOffsetLV(agricultureProject.PublishDate),
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
