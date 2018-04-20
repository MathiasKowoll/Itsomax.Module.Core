using MediatR;

namespace Itsomax.Module.Core.Events
{
    public class ReviewSummaryChanged : INotification
    {
        public long EntityId { get; set; }

        public long EntityTypeId { get; set; }

        public int ReviewsCount { get; set; }

        public double? RatingAverage { get; set; }
    }
}
