using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    public interface IDocService
    {
        Task<Document> Down(int entityId);
        Task<Document> CopyDocumentAsync(int sourceId, int targetFolderId);
        Task SynchronizeDocument(int sourceId, int targetId);
    }
}