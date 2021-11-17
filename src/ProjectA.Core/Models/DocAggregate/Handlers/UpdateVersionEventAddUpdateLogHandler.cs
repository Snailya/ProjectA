using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class UpdateVersionEventAddUpdateLogHandler : INotificationHandler<UpdateVersionEvent>
    {
        public Task Handle(UpdateVersionEvent notification, CancellationToken cancellationToken)
        {
            foreach (var set in notification.Document.Sets) set.UpdateDocument(notification.Document);

            return Task.CompletedTask;
        }
    }
}