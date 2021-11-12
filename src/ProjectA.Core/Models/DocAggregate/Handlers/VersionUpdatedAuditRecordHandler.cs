using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class VersionUpdatedAuditRecordHandler : INotificationHandler<VersionUpdatedEvent>
    {
        private readonly IDocSetService _docSetService;

        public VersionUpdatedAuditRecordHandler(IDocSetService docSetService)
        {
            _docSetService = docSetService;
        }

        public Task Handle(VersionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _docSetService.AddUpdateLog(notification.Document);
        }
    }
}