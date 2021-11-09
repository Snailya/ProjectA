using System;
using System.Threading.Tasks;

namespace ProjectA.Core.Interfaces
{
    public interface IDocSetLogService
    {
        Task AddUpdateLog(Guid guid);
    }
}