using System.Linq;
using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentsWithBindingsSpec : Specification<Document>
    {
        public DocumentsWithBindingsSpec()
        {
            Query
                .Include(document => document.Bindings)
                .Where(document => document.Bindings.Any());
        }
    }
}