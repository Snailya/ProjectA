using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class InitDocumentEventUpdateInfoHandler : INotificationHandler<InitDocumentEvent>
    {
        private readonly IDocumentHelper _documentHelper;

        public InitDocumentEventUpdateInfoHandler(IDocumentHelper documentHelper)
        {
            _documentHelper = documentHelper;
        }


        public Task Handle(InitDocumentEvent notification, CancellationToken cancellationToken)
        {
            return _documentHelper.UpdateInfo(notification.Document);
        }
    }
}