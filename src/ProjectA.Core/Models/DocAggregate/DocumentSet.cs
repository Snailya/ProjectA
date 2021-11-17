using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ProjectA.SharedKernel;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Core.Models.DocAggregate
{
    public class DocumentSet : BaseEntity, IAggregateRoot
    {
        private readonly List<Document> _documents = new();
        private readonly List<DocumentSetLog> _logs = new();

        public string Name { get; set; }

        public IReadOnlyCollection<Document> Documents =>
            _documents.AsReadOnly(); // one to zero many relationship

        public ReadOnlyCollection<DocumentSetLog> Logs => _logs.AsReadOnly();

        #region Public Methods

        public void AddDocument(Document document)
        {
            if (_documents.Contains(document))
                throw new InvalidOperationException("Document has already included in the set.");

            _documents.Add(document);
            _logs.Add(new DocumentSetLog(document, DocumentSetLogType.Created));
        }

        public void UpdateDocument(Document document)
        {
            if (!_documents.Contains(document))
                throw new InvalidOperationException("Document is not in the set please add it first.");

            var lastLog = _logs.Last(x => x.Document == document);
            if (lastLog.Version != document.CurVersion)
                _logs.Add(new DocumentSetLog(document, DocumentSetLogType.Updated));
        }

        public void RemoveDocument(Document document)
        {
            if (!_documents.Contains(document))
                throw new InvalidOperationException("Document has already been removed from the set.");

            _documents.Remove(document);
            _logs.Add(new DocumentSetLog(document, DocumentSetLogType.Deleted));
        }

        public IEnumerable<DocumentTrackingRecord> GenerateTrackingReports(DateTime from)
        {
            var records = new List<DocumentTrackingRecord>();
            var logLookUp = _logs.Where(x => x.Time >= from).ToLookup(x => x.Document);
            foreach (var logGroup in logLookUp)
            {
                if (logGroup.First().Type == DocumentSetLogType.Created)
                {
                    if (logGroup.Last().Type != DocumentSetLogType.Deleted)
                        records.Add(new DocumentTrackingRecord(logGroup.Last().Document.FileName,
                            logGroup.Last().Version.VersionNumber.ToString(), DocumentSetLogType.Created));
                    continue;
                }

                records.Add(new DocumentTrackingRecord(logGroup.Last().Document.FileName,
                    logGroup.Last().Version.VersionNumber.ToString(), DocumentSetLogType.Updated));
            }

            return records;
        }

        #endregion
    }
}