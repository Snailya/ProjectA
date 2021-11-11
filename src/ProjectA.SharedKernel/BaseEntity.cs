using System;
using System.Collections.Generic;

namespace ProjectA.SharedKernel
{
    public abstract class BaseEntity
    {
        public Guid Guid { get; set; }

        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
    }
}