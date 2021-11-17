using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    public interface IAppService
    {
        Task ListenAndUpdate();
        Task<Document> AddDocument(int entityId);
        Task<Document> AddDocumentBindingToFolder(int sourceId, int targetFolderId);
        Task<Document> AddDocumentBindingToExistFile(int sourceId, int targetId);
    }
}