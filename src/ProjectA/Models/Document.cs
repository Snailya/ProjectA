#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectA.Models
{
    public class Document
    {
        #region Private Properties

        private readonly List<DocVersion> _versions = new();

        #endregion

        public void SetSnapshotFolder(int snapshotFolderId)
        {
            if (Snapshot != null)
                throw new InvalidOperationException(
                    "Change snapshot folder is not allowed if the document already has a linked snapshot. " +
                    "The snapshot folder id is only used the first time synchronizing the document from source folder to target folder. " +
                    "After the snapshot has created, the tracking process is going with the snapshot's entity id.");

            SnapshotFolderId = snapshotFolderId;
        }

        public void UpdateSnapshot(Document newSnapshot)
        {
            Snapshot = newSnapshot;
        }

        public void UpdateVersion(DocVersion newVersion)
        {
            if (_versions.Any(x => x.VersionNumber == newVersion.VersionNumber))
                throw new InvalidOperationException(
                    "A document can't have two version with the same version number.");

            _versions.Add(newVersion);
        }

        #region Public Properties

        #region Relationships

        public Document? Snapshot { get; private set; } // one to zero one relationship
        public IEnumerable<DocVersion> Versions => _versions.AsReadOnly(); // one to zero many relationship

        #endregion

        public Guid Guid { get; private set; } // used by efcore to trace state

        public int EntityId { get; private set; } // XXX: private set is used by efcore

        public int SnapshotFolderId { get; private set; }

        #endregion

        #region Constructors

        public Document(int entityId, int snapshotFolderId)
        {
            EntityId = entityId;
            SnapshotFolderId = snapshotFolderId;
        }

        public Document(int entityId)
        {
            EntityId = entityId;
        }

        #endregion
    }
}