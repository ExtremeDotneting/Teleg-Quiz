using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public class UserEntityStore:Store<UserEntity, int>, IUserEntityStore
    {
        public UserEntityStore(Func<LiteDatabase> dbProvider) : base(dbProvider)
        {
            using (var db = GetDatabase())
            {
                var collection = db.GetCollection<UserEntity>();
                collection.EnsureIndex(nameof(UserEntity.MessengerUsername));
            }
        }
    }
}
