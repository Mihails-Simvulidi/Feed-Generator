using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedGenerator
{
    public class FeedActionResult : ActionResult
    {
        private readonly SyndicationFeed _feed;

        public FeedActionResult(SyndicationFeed feed)
        {
            _feed = feed;
        }

        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            Rss20FeedFormatter formatter = new Rss20FeedFormatter(_feed);
            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                //Indent = true,
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(context.HttpContext.Response.Body, xmlSettings))
            {
                formatter.WriteTo(xmlWriter);
            }
        }
    }
}
