using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate.Events
{
    public class AddToSetEvent : BaseDomainEvent
    {
        public AddToSetEvent(Document document, DocumentSet set)
        {
            Document = document;
            Set = set;
        }

        public Document Document { get; set; }
        public DocumentSet Set { get; set; }
    }
}