using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class AddToSetEventCreateAddLogHandler : INotificationHandler<AddToSetEvent>
    {
        public Task Handle(AddToSetEvent notification, CancellationToken cancellationToken)
        {
            notification.Set.AddDocument(notification.Document);
            return Task.CompletedTask;
        }
    }
}