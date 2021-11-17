using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentSpec : Specification<Document>, ISingleResultSpecification
    {
        public DocumentSpec(int entityId)
        {
            Query
                .Where(document => document.EntityId == entityId);
        }
    }
}