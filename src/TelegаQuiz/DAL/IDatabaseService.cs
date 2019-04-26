using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL
{
    /// <summary>
    /// Added "Service" because IDatabase very often used in other libraries.
    /// </summary>
    public interface IDatabaseService
    {
        IDbSet<TEntity> Repository<TEntity>() 
            where TEntity : BaseEntity;

    }
}
