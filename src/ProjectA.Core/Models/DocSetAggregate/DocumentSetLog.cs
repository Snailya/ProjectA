using System;

namespace ProjectA.Core.Models.DocSetAggregate
{
    public record DocumentSetLog(Guid Document, DateTime Time, DocumentSetLogType Type)
    {
    }
}