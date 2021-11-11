using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectA.SharedKernel;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Core.Models.DocSetAggregate
{
    public class DocumentSet : BaseEntity,IAggregateRoot
    {
        private readonly List<DocumentSetLog> _logs = new();

        #region Relationships

        public List<Guid> Documents { get; set; } = new();

        #endregion

        public string Name { get; set; }

        public ReadOnlyCollection<DocumentSetLog> Logs => _logs.AsReadOnly();


        #region Public Methods

        public void Add(Guid document)
        {
            if (Documents.Contains(document)) return;

            Documents.Add(document);
            AddLog(document, DocumentSetLogType.Added);
        }

        public void Update(Guid document)
        {
            if (!Documents.Contains(document))
                throw new InvalidOperationException("Can update a non-exist document of the document set.");

            AddLog(document);
        }

        public void Delete(Guid document)
        {
            if (!Documents.Contains(document))
                throw new InvalidOperationException("Can not delete a non-exist document from document set.");

            Documents.Remove(document);
            AddLog(document, DocumentSetLogType.Deleted);
        }

        private void AddLog(Guid document, DocumentSetLogType logType = DocumentSetLogType.Updated)
        {
            _logs.Add(new DocumentSetLog(document, DateTime.UtcNow, logType));
        }

        #endregion
    }
}