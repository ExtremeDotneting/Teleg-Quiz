using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegаQuiz.DAL;
using TelegаQuiz.Entities;

namespace TelegаQuiz.BL
{
    public class GameLogicService : IGameLogicService
    {
        readonly IDatabaseService _db;

        public GameLogicService(IDatabaseService db)
        {
            _db = db;
        }

        public async Task<Question> NewQuestion()
        {
            return await _db
                .StoreQuestions()
                .GetRandomQuestionAsync();
        }

        public async Task<ChatStats> GetChatStats(string messengerUsername, string chatId)
        {
            var store = _db.StoreChatStats();
            var chatStats = await store.FindByChatIdAsync(chatId);
            if (chatStats == null)
            {
                chatStats = new ChatStats();
                chatStats.ChatId = chatId;
                var currentUserStats = new UserStats()
                {
                    MessengerUsername = messengerUsername
                };
                chatStats.UsersStats = new List<UserStats>() { currentUserStats };
                await store.UpsertAsync(chatStats);
            }
            return chatStats;
        }

        public async Task<UserStats> GetUserStats(string messengerUsername, string chatId)
        {
            var chatStats = await GetChatStats(messengerUsername, chatId);
            var userStats = chatStats.UsersStats.First(u => u.MessengerUsername == messengerUsername);
            return userStats;
        }

        public bool IsStateEmpty(GameState state)
        {
            return state.Question == null;
        }

        public bool CanAskNewQuestion(GameState state)
        {
            return DateTime.UtcNow - state.LastQuestionUpdate > GameConstants.RequestNewQuestionTimeout;
        }

        public TimeSpan TimeToNextRequest(GameState state)
        {
            return GameConstants.RequestNewQuestionTimeout - (DateTime.UtcNow - state.LastQuestionUpdate);
        }

        public async Task<(bool IsRight, UserStats Stats)> CheckAnswerAndUpdateStats(
            string text,
            GameState state,
            string messengerUsername,
            string chatId
            )
        {
            var store = _db.StoreChatStats();
            var chatStats = await store.FindByChatIdAsync(chatId);
            if (chatStats == null)
            {
                chatStats = new ChatStats();
                chatStats.ChatId = chatId;
                chatStats.UsersStats = new List<UserStats>() { };
            }
            var userStats = chatStats.UsersStats.FirstOrDefault(u => u.MessengerUsername == messengerUsername);
            if (userStats == null)
            {
                userStats = new UserStats()
                {
                    MessengerUsername = messengerUsername
                };
                chatStats.UsersStats.Add(userStats);
            }

            userStats.TotalAnswersCount++;
            var isRight = IsAnswerRight(text, state);
            if (isRight)
            {
                userStats.CorrectAnswersCount++;
            }
            await store.UpsertAsync(chatStats);
            return (isRight, userStats);
        }

        public bool IsAnswerRight(string text, GameState state)
        {
            //I am too lazy to optimize this. Just check substring in answer.
            return text.Trim().ToLower().Contains(
                    state.Question.AnswerText.ToLower().Trim()
                    );
        }
    }
}
