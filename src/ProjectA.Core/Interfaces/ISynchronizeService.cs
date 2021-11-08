using System.Threading.Tasks;

namespace ProjectA.Core.Interfaces
{
    public interface ISynchronizeService
    {
        Task<int> Down();
        Task<int> Up();
    }
}