using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL
{
    public interface IStore<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        int Count { get; }

        Task<TEntity> FindByIdAsync(TKey id);

        Task UpsertAsync(TEntity entity);

        Task<TKey> InsertAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(TKey id);


    }
}
