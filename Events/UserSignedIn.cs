using MediatR;

namespace Itsomax.Module.Core.Events
{
    public class UserSignedIn : INotification
    {
        public long UserId { get; set; }
    }
}
