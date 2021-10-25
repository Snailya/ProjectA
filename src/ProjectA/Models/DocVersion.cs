using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProjectA.Models
{
    [Owned]
    public record DocVersion
    {
        [Key]public int VersionId { get; init; }
        public VersionNumber VersionNumber { get; init; }

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