using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.DAL
{
    public interface IChatStatsStore : IStore<ChatStats, int>
    {
        Task<ChatStats> FindByChatIdAsync(string chatId);
    }
}
