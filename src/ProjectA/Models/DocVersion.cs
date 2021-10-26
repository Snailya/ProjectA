namespace ProjectA.Models
{
    public record DocVersion
    {
        public int VersionId { get; init; }
        public VersionNumber VersionNumber { get; init; }
    }
}