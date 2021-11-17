using System;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate.Events
{
    public class UpdateVersionEvent : BaseDomainEvent
    {
        public UpdateVersionEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; set; }
    }
}