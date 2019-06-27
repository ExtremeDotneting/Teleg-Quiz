using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using TelegаQuiz.Entities;
using TelegаQuiz.Exceptions;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public class Store<TEntity, TKey> : IStore<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        readonly Func<LiteDatabase> _dbProvider;

        public Store(Func<LiteDatabase> dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public int Count {
            get
            {
                using (var db = GetDatabase())
                {
                    var collection = db.GetCollection<TEntity>();
                    return collection.Count();
                }
            }
        }

        public virtual async Task<TEntity> FindByIdAsync(TKey id)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var idBson = new BsonValue(id);
                    var res = db.GetCollection<TEntity>()
                        .FindById(idBson);
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        public virtual async Task UpsertAsync(TEntity entity)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var isInsert = db.GetCollection<TEntity>()
                        .Upsert(entity);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        public virtual async Task<TKey> InsertAsync(TEntity entity)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var id = db.GetCollection<TEntity>()
                        .Insert(entity);
                    return (TKey)(object)id;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    db.GetCollection<TEntity>()
                        .Update(entity);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }


        public virtual async Task DeleteAsync(TEntity entity)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var idBson = new BsonValue(entity.Id);
                    db.GetCollection<TEntity>()
                        .Delete(idBson);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var idBson = new BsonValue(id);
                    db.GetCollection<TEntity>()
                        .Delete(idBson);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        protected LiteDatabase GetDatabase()
        {
            return _dbProvider();
        }
    }


}
