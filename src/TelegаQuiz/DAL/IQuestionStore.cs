using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL
{
    public interface IQuestionStore : IStore<Question, int>
    {
        Task<Question> GetRandomQuestionAsync();
    }
}
