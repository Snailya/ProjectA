using System;
using MediatR;

namespace ProjectA.Core.Shared
{
    public abstract class BaseDomainEvent : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}