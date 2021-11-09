using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class VersionUpdatedAuditRecordHandler : INotificationHandler<VersionUpdatedEvent>
    {
        private readonly IDocSetLogService _docSetLogService;

        public VersionUpdatedAuditRecordHandler(IDocSetLogService docSetLogService)
        {
            _docSetLogService = docSetLogService;
        }

        public Task Handle(VersionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return _docSetLogService.AddUpdateLog(notification.Guid);
        }
    }
}