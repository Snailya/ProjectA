#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectA.Models
{
    public class Document
    {
        private readonly List<DocVersion> _versions = new();

        public Document(int entityId, int snapshotFolderId)
        {
            EntityId = entityId;
            SnapshotFolderId = snapshotFolderId;
        }

        public Document(int entityId)
        {
            EntityId = entityId;
        }

        [Key] public int EntityId { get; private set; }  // private set is used by efcore

        // navigation property
        public Document? Snapshot { get; private set; }
        public int SnapshotFolderId { get; }

        public bool HasSnapshot => SnapshotFolderId is not default(int);

        // owns
        public IEnumerable<DocVersion> Versions => _versions.AsReadOnly();

        public void SetSnapshot(Document snapshot)
        {
            Snapshot ??= snapshot;

            throw new Exception($"This document is already linked to a document with id {Snapshot.EntityId}");
        }

        public void UpdateVersion(DocVersion newVersion)
        {
            _versions.Add(newVersion);
        }
    }
}