using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate.Events;

namespace ProjectA.Core.Models.DocAggregate.Handlers
{
    public class AddBindingEventUpdateSpecificBindingHandler : INotificationHandler<AddBindingEvent>
    {
        private readonly IDocumentHelper _documentHelper;

        public AddBindingEventUpdateSpecificBindingHandler(IDocumentHelper documentHelper)
        {
            _documentHelper = documentHelper;
        }


        public Task Handle(AddBindingEvent notification, CancellationToken cancellationToken)
        {
            return _documentHelper.UpdateBindings(notification.Document, notification.Binding);
        }
    }
}