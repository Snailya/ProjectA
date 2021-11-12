using System;
using System.Collections.Generic;

namespace ProjectA.SharedKernel
{
    public abstract class BaseEntity
    {
        public List<BaseDomainEvent> Events = new();
        public Guid Guid { get; set; }
    }
}