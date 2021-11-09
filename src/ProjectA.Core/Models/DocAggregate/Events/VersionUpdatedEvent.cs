using System;
using ProjectA.Core.Shared;

namespace ProjectA.Core.Models.DocAggregate.Events
{
    public class VersionUpdatedEvent : BaseDomainEvent
    {
        public VersionUpdatedEvent(Guid id)
        {
            Guid = id;
        }

        public Guid Guid { get; set; }
    }
}