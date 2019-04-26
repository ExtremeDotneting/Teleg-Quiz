using System.Collections.Generic;
using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public class LiteDbSet<TEntity> : IDbSet<TEntity>
        where TEntity : BaseEntity
    {
        public Task<TEntity> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<TEntity>> GetMany(int limit)
        {
            throw new System.NotImplementedException();
        }

        public Task Upsert(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }


}
