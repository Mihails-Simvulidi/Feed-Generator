using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedGenerator
{
    public class FeedActionResult : ActionResult
    {
        SyndicationFeed _feed;

        public FeedActionResult(SyndicationFeed feed)
        {
            _feed = feed;
        }

        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var formatter = new Rss20FeedFormatter(_feed);
            var xmlSettings = new XmlWriterSettings
            {
                //Indent = true,
            };
            using (var xmlWriter = XmlWriter.Create(context.HttpContext.Response.Body, xmlSettings))
                formatter.WriteTo(xmlWriter);
        }
    }
}
