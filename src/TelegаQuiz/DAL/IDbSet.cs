using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL
{
    public interface IDbSet<TEntity>
        where TEntity:BaseEntity
    {
        Task<TEntity> Get(int id);

        Task<ICollection<TEntity>> GetMany(int limit);

        Task Upsert(TEntity entity);

        Task Delete(TEntity entity);

        Task Delete(int id);
    }
}
