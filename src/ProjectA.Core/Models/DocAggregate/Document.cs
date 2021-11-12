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
        #region Private Properties

        private readonly List<DocumentVersion> _versions = new();

        #endregion

        #region Public Methods

        public void SetLinkedDocFolderId(int snapshotFolderId)
        {
            if (LinkedDoc != null)
                throw new InvalidOperationException(
                    "Change snapshot folder is not allowed if the document already has a linked snapshot. " +
                    "The snapshot folder id is only used the first time synchronizing the document from source folder to target folder. " +
                    "After the snapshot has created, the tracking process is going with the snapshot's entity id.");

            LinkedDocFolderId = snapshotFolderId;
        }

        public void MakeLink(Document linkedDoc)
        {
            LinkedDoc = linkedDoc;
        }

        public void UpdateVersion(DocumentVersion newDocumentVersion)
        {
            if (_versions.Any(x => x.VersionNumber == newDocumentVersion.VersionNumber))
                throw new InvalidOperationException(
                    "A document can't have two version with the same version number.");

            _versions.Add(newDocumentVersion);
            Events.Add(new VersionUpdatedEvent(this));
        }

        #endregion

        #region Public Properties

        #region Relationships

        public Document? LinkedDoc { get; private set; } // one to zero one relationship

        public IReadOnlyCollection<DocumentVersion> Versions =>
            _versions.AsReadOnly(); // one to zero many relationship

        #endregion

        public int EntityId { get; private set; } // XXX: private set is used by efcore
        public int LinkedDocFolderId { get; private set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileNamePath { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public DocumentVersion? CurVersion => Versions.LastOrDefault();
        
        // public bool LinkedDocNeedUpdate =>
        //     (CurVersion == null ? new DocumentVersionNumber(0, 0) : CurVersion.VersionNumber) >
        //     (LinkedDoc?.CurVersion == null
        //         ? new DocumentVersionNumber(0, 0)
        //         : LinkedDoc?.CurVersion.VersionNumber) && LinkedDocFolderId != 0;

        #endregion

        #region Constructors

        public Document(int entityId, int linkedDocFolderId)
        {
            EntityId = entityId;
            LinkedDocFolderId = linkedDocFolderId;
        }

        public Document(int entityId)
        {
            EntityId = entityId;
        }

        #endregion
    }
}