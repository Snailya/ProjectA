using Ardalis.Specification;

namespace ProjectA.Core.Models.DocAggregate.Specifications
{
    public class DocumentThatNeedUpdateLinkedDocSpec:Specification<Document>
    {
        public DocumentThatNeedUpdateLinkedDocSpec()
        {
            Query
                .Include(document => document.LinkedDoc).Where(x=>x.LinkedDocNeedUpdate);
        }
    }
}