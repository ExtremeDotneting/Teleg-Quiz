using System;
using System.Collections.Generic;
using LiteDB;
using TelegаQuiz.Entities;
using TelegаQuiz.Exceptions;

namespace TelegаQuiz.DAL.OnLiteDB
{

    public class LiteDatabaseService : IDatabaseService
    {
        readonly Func<LiteDatabase> _dbProvider;

        /// <summary>
        /// Called on app startup.
        /// </summary>
        public LiteDatabaseService(string connectionString)
        {
            _dbProvider = () => new LiteDatabase(connectionString);
        }

        public IStore<TEntity, TKey> Store<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var customStoreEntities = new List<Type>
            {
                typeof(Question),
                typeof(ChatStats),
                typeof(UserEntity)
            };
            if (customStoreEntities.Contains(typeof(TEntity)))
            {
                throw new DatabaseException($"You must use another method to get custom Store for entity {typeof(TEntity).Name}.");
            }

            var store = new Store<TEntity, TKey>(_dbProvider);
            return store;
        }

        public IQuestionStore StoreQuestions()
        {
            var store= new QuestionStore(_dbProvider);
            return store;
        }

        public IChatStatsStore StoreChatStats()
        {
            var store = new ChatStatsStore(_dbProvider);
            return store;
        }

        public IUserEntityStore StoreUserEntity()
        {
            var store = new UserEntityStore(_dbProvider);
            return store;
        }
    }


}
