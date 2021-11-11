using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentByEntityIdSpec : Specification<Document>, ISingleResultSpecification
    {
        public DocumentByEntityIdSpec(int entityId)
        {
            Query
                .Where(document => document.EntityId == entityId);
        }
    }
}