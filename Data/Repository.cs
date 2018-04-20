using Itsomax.Data.Infrastructure.Data;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Data
{
    public class Repository<T> : RepositoryWithTypedId<T, long>, IRepository<T>
       where T : class, IEntityWithTypedId<long>
    {
        public Repository(ItsomaxDbContext context) : base(context)
        {
        }
    }
}