using Ardalis.Specification;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Models.DocSetAggregate.Specifications
{
    public class DocumentSetContainsSpecificDocumentSpec : Specification<DocumentSet>
    {
        public DocumentSetContainsSpecificDocumentSpec(Document document)
        {
            Query
                .Where(set => set.Documents.Contains(document));
        }
    }
}