using System;
using ProjectA.SharedKernel;

namespace ProjectA.Core.Models.DocAggregate
{
    public class DocumentVersion : BaseEntity, IComparable<DocumentVersion>
    {
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