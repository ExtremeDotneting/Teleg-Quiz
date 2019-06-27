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
        IStore<TEntity, TKey> Store<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>;

        IQuestionStore StoreQuestions();

        IChatStatsStore StoreChatStats();

        IUserEntityStore StoreUserEntity();

    }
}
