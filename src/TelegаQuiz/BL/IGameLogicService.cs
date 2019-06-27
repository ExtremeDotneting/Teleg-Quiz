using System;
using System.Threading.Tasks;
using TelegаQuiz.Entities;

namespace TelegаQuiz.BL
{
    public interface IGameLogicService
    {
        Task<Question> NewQuestion();

        Task<ChatStats> GetChatStats(string messengerUsername, string chatId);

        Task<UserStats> GetUserStats(string messengerUsername, string chatId);

        bool IsStateEmpty(GameState state);

        bool CanAskNewQuestion(GameState state);

        TimeSpan TimeToNextRequest(GameState state);

        Task<(bool IsRight, UserStats Stats)> CheckAnswerAndUpdateStats(
            string text,
            GameState state,
            string messengerUsername,
            string chatId
            );

        bool IsAnswerRight(string text, GameState state);
    }
}