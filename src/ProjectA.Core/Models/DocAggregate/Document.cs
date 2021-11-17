#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectA.Core.Models.DocAggregate.Events;
using ProjectA.SharedKernel;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Core.Models.DocAggregate
{
    public class Document : BaseEntity, IAggregateRoot
    {
        #region Constructors

        public Document(int entityId)
        {
            EntityId = entityId;
            Events.Add(new InitDocumentEvent(this));
        }

        #endregion

        #region Private Properties

        private readonly List<DocumentVersion> _versions = new();
        private readonly List<Document> _bindings = new();
        private readonly List<DocumentSet> _sets = new();

        #endregion

        #region Public Methods

        public void AddBinding(Document newBinding)
        {
            if (_bindings.All(x => x.EntityId == newBinding.EntityId)) return;

            _bindings.Add(newBinding);
            Events.Add(new AddBindingEvent(this, newBinding));
        }

        public void UpdateVersion(DocumentVersion newDocumentVersion)
        {
            if (_versions.Any(x => x.VersionNumber == newDocumentVersion.VersionNumber))
                throw new InvalidOperationException(
                    "A document can't have two version with the same version number.");

            _versions.Add(newDocumentVersion);
            Events.Add(new UpdateVersionEvent(this));
        }

        public void AddToSet(DocumentSet set)
        {
            if (_sets.Contains(set))
                throw new InvalidOperationException("This document is already included in the set.");
            _sets.Add(set);
            Events.Add(new AddToSetEvent(this, set));
        }

        #endregion

        #region Public Properties

        public IEnumerable<DocumentVersion> Versions =>
            _versions.AsReadOnly(); // one to zero many relationship

        public IReadOnlyCollection<Document> Bindings =>
            _bindings.AsReadOnly(); // one to zero many relationship

        public IEnumerable<DocumentSet> Sets =>
            _sets.AsReadOnly(); // one to zero many relationship


        #region Basic Info

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileNamePath { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }

        #endregion

        public int EntityId { get; private set; } // XXX: private set is used by efcore

        public DocumentVersion? CurVersion => Versions.LastOrDefault();

        #endregion
    }
}