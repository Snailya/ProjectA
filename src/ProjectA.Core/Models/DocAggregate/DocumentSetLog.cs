using System;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate
{
    public class DocumentSetLog : BaseEntity
    {
        public DocumentSetLog(Document document, DocumentSetLogType type)
        {
            Document = document;
            Version = document.CurVersion;
            Time = DateTime.UtcNow;
            Type = type;
        }

        public DateTime Time { get; init; }
        public DocumentSetLogType Type { get; init; }
        public Document Document { get; init; }
        public DocumentVersion Version { get; init; }
    }
}