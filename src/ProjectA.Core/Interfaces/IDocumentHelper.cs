using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    /// <summary>
    /// Help to solve dms related unit of works.
    /// </summary>
    public interface IDocumentHelper
    {
        Task UpdateInfo(Document document);
        Task<Document> CreateBinding(Document document, int folderId);
        Task UpdateBindings(Document document, Document specificBinding = null);
    }
}