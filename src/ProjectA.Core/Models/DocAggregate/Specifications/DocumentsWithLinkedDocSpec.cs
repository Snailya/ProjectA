using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentsWithLinkedDocSpec : Specification<Document>
    {
        public DocumentsWithLinkedDocSpec()
        {
            Query
                .Where(document => document.LinkedDocFolderId != default)
                .Include(document => document.LinkedDoc);
        }
    }
}