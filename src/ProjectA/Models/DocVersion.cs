using System;

namespace ProjectA.Models
{
    public record DocVersion
    {
        public Guid Guid { get; set; } // used by efcore to as foreign key
        public int VersionId { get; init; }
        public VersionNumber VersionNumber { get; init; }
    }
}