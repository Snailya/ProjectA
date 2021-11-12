using System;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocSetAggregate
{
    public class DocumentSetLog : BaseEntity
    {
        public DocumentSetLog(Guid documentId, DateTime time, DocumentSetLogType type)
        {
            DocumentId = documentId;
            Time = time;
            Type = type;
        }

        public Guid DocumentId { get; init; }
        public DateTime Time { get; init; }
        public DocumentSetLogType Type { get; init; }
    }
}