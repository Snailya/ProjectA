using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    public interface IDMSService
    {
        Document GetDocument(int fileId);
        IEnumerable<DocumentVersion> GetVersions(int fileId);
        int CopySingleDocument(int sourceId, int targetFolderId);
        Task<MemoryStream> DownloadVersionAsync(int versionId);
        Task<int> UpdateVersionAsync(int fileId, Stream fileStream);
        void PublishVersion(int fileId);
    }
}