using System;

namespace ProjectA.Models
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
    }
}