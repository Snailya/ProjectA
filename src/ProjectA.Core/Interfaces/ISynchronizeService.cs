using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    public interface ISynchronizeService
    {
        Task BatchDownAsync();
        Task BatchSynchronizeDocumentsAsync();
        Task<Document> Down(int entityId);
        Task<Document> CopyDocumentAsync(int sourceId, int targetFolderId);
        Task SynchronizeDocument(int sourceId, int targetId);
    }
}