using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class UpdateVersionEventUpdateBindingsHandler : INotificationHandler<UpdateVersionEvent>
    {
        private readonly IDocumentHelper _documentHelper;

        public UpdateVersionEventUpdateBindingsHandler(IDocumentHelper documentHelper)
        {
            _documentHelper = documentHelper;
        }

        public Task Handle(UpdateVersionEvent notification, CancellationToken cancellationToken)
        {
            return _documentHelper.UpdateBindings(notification.Document);
        }
    }
}