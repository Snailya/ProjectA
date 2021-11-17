using System;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate.Events
{
    public class AddBindingEvent : BaseDomainEvent
    {
        public AddBindingEvent(Document document, Document binding)
        {
            Document = document;
            Binding = binding;
        }

        public Document Binding { get; set; }

        public Document Document { get; set; }
    }
}