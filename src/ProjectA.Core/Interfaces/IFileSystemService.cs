using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    /// <summary>
    ///     Infrastructure dependency interface.
    /// </summary>
    public interface IFileSystemService
    {
        Document GetDocument(int id);
        IEnumerable<DocumentVersion> GetVersions(Document document);
        Document CopySingleDocument(Document source, int targetFolderId);
        Task<MemoryStream> DownloadVersionAsync(int versionId);
        Task<DocumentVersion> UpdateVersionAsync(Document document, MemoryStream fileStream);
        Task<DocumentVersion> PublishVersion(Document document);
    }
}