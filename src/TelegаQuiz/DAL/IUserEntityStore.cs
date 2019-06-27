using TelegаQuiz.DAL.OnLiteDB;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL
{
    public interface IUserEntityStore: IStore<UserEntity, int>
    {
    }
}