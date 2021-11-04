using System;

namespace ProjectA.Core.Models
{
    public record DocVersion : IComparable<DocVersion>
    {
        public Guid Guid { get; set; } // used by efcore to as foreign key
        public int VersionId { get; init; }
        public VersionNumber VersionNumber { get; init; }

        public int CompareTo(DocVersion other)
        {
            return VersionNumber < other.VersionNumber ? -1 : VersionNumber == other.VersionNumber ? 0 : 1;
        }

        public static bool operator >(DocVersion a, DocVersion b)
        {
            return a.VersionNumber > b.VersionNumber;
        }

        public static bool operator <(DocVersion a, DocVersion b)
        {
            return a.VersionNumber < b.VersionNumber;
        }
    }
}