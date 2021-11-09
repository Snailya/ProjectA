using System;

namespace ProjectA.Core.Models.DocAggregate
{
    public record DocumentVersion : IComparable<DocumentVersion>
    {
        public Guid Guid { get; set; } // used by efcore to as foreign key
        public int VersionId { get; init; }
        public DocumentVersionNumber VersionNumber { get; init; }

        public int CompareTo(DocumentVersion other)
        {
            return VersionNumber < other.VersionNumber ? -1 : VersionNumber == other.VersionNumber ? 0 : 1;
        }

        public static bool operator >(DocumentVersion a, DocumentVersion b)
        {
            return a.VersionNumber > b.VersionNumber;
        }

        public static bool operator <(DocumentVersion a, DocumentVersion b)
        {
            return a.VersionNumber < b.VersionNumber;
        }
    }
}