using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentByEntityIdWithLinkedDocSpec : Specification<Document>, ISingleResultSpecification
    {
        public DocumentByEntityIdWithLinkedDocSpec(int entityId)
        {
            Query
                .Where(document => document.EntityId == entityId)
                .Include(document => document.LinkedDoc);
        }
    }
}