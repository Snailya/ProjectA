using System.Threading.Tasks;

namespace ProjectA.Core.Interfaces
{
    public interface IShepherdService
    {
        Task<int> SyncDocVersionsFromEDocAsync();
        Task<int> UpdateSnapshotInTargetFolderAsync();
    }
}