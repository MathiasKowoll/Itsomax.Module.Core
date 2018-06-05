using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Data
{
    public interface ICustomRepositoryFunctions : IRepository<Entity>
    {
        byte[] SetEncryption(string stringToEncrypt);
        string GetDecryption(byte[] stringToDecrypt);
    }
}