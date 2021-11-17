using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate.Events
{
    public class InitDocumentEvent : BaseDomainEvent
    {
        public InitDocumentEvent(Document document)
        {
            Document = document;
        }
        
        public Document Document { get; set; }
    }
}