using System;
using System.Threading.Tasks;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Interfaces
{
    public interface IDocSetService
    {
        Task AddUpdateLog(Document document);
    }
}