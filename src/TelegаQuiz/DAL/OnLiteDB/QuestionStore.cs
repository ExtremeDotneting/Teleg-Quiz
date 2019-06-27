using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using TelegаQuiz.Entities;
using TelegаQuiz.Exceptions;

namespace TelegаQuiz.DAL.OnLiteDB
{
    public class QuestionStore : Store<Question, int>, IQuestionStore
    {
        readonly object _restoreLocker = new object();

        public QuestionStore(Func<LiteDatabase> dbProvider) : base(dbProvider)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var collection = db.GetCollection<Question>();
                    if (collection.Count() == 0)
                    {
                        collection.Insert(new Question
                        {
                            QuestionText =
                                "Current question was automatically added when db initialized. Used for tests. Answer is \"ok\"",
                            AnswerText="ok"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        public async Task<Question> GetRandomQuestionAsync()
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var collection = db.GetCollection<Question>();
                    var rd = new Random();
                    lock (_restoreLocker) { }

                    var id = rd.Next(collection.Min("_id"), collection.Max("_id"));
                    var question = collection.FindById(id);

                    //If can't find by random id.
                    //Not good, but example not about database.
                    if (question == null)
                    {
                        RestoreQuestionsId();
                    }
                    id = rd.Next(collection.Min("_id"), collection.Max("_id"));
                    question = collection.FindById(id);
                    return question;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("", ex);
            }
        }

        void RestoreQuestionsId()
        {
            lock (_restoreLocker)
            {
                using (var db = GetDatabase())
                {
                    var collection = db.GetCollection<Question>();
                    var list = collection.FindAll().ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Id = i;
                    }
                    db.DropCollection(nameof(Question));
                    collection = db.GetCollection<Question>();
                    collection.Insert(list);
                }
            }
        }
    }
}
