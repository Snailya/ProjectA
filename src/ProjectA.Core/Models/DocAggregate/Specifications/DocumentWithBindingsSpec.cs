using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentWithBindingsSpec : Specification<Document>, ISingleResultSpecification
    {
        public DocumentWithBindingsSpec(int entityId)
        {
            Query
                .Where(document => document.EntityId == entityId)
                .Include(document => document.Bindings);
        }
    }
}