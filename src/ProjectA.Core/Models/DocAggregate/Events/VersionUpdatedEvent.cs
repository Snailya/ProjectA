using System;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate.Events
{
    public class VersionUpdatedEvent : BaseDomainEvent
    {
        public VersionUpdatedEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; set; }
    }
}