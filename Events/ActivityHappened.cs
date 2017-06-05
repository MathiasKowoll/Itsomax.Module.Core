using System;
using MediatR;

namespace Itsomax.Module.Core.Events
{
    public class ActivityHappened : INotification
    {
        public long ActivityTypeId { get; set; }

        public long EntityId { get; set; }

        public long EntityTypeId { get; set; }

        public DateTimeOffset TimeHappened { get; set; }
    }
}
