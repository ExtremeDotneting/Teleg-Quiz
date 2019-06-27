using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using TelegаQuiz.Entities;
using TelegаQuiz.Exceptions;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public class ChatStatsStore : Store<ChatStats, int>, IChatStatsStore
    {
        public ChatStatsStore(Func<LiteDatabase> dbProvider) : base(dbProvider)
        {
        }

        public async Task<ChatStats> FindByChatIdAsync(string chatId)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var res = db.GetCollection<ChatStats>()
                        .FindOne(r => r.ChatId == chatId);
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }
    }
}
