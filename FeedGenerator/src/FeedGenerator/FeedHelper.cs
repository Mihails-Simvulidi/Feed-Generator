using System;
using System.ServiceModel.Syndication;

namespace FeedGenerator
{
    internal static class FeedHelper
    {
        private static Lazy<TimeZoneInfo> TimeZoneLV => new Lazy<TimeZoneInfo>(() => TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

        public static DateTimeOffset GetDateTimeOffsetLV(DateTime dateTimeLocal)
        {
            TimeSpan utcOffset = TimeZoneLV.Value.GetUtcOffset(dateTimeLocal);
            return new DateTimeOffset(dateTimeLocal, utcOffset);
        }

        public static void AddExceptionToFeed(SyndicationFeed feed, Exception e)
        {
            DateTime nowLV = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneLV.Value);

            SyndicationItem syndicationItem = new SyndicationItem()
            {
                Title = new TextSyndicationContent("Nevarēju dabūt publikāciju sarakstu"),
                Content = new TextSyndicationContent($"{e.GetType()}: {e.Message}"),
                PublishDate = GetDateTimeOffsetLV(nowLV.Date),
            };

            feed.Items = new SyndicationItem[] { syndicationItem };
        }
    }
}
