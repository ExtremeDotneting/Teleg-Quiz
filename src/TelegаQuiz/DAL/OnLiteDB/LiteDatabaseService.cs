using System;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL.OnLiteDB
{

    public class LiteDatabaseService : IDatabaseService
    {
        public IDbSet<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }
    }


}
