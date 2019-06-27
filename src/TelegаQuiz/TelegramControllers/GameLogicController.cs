using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types.Enums;
using TelegаQuiz.BL;
using TelegаQuiz.DAL;
using TelegаQuiz.Entities;

namespace TelegаQuiz.TelegramControllers
{
    public class GameLogicController : BotController
    {
        const string GameStateKey = "GameState";

        readonly IGameLogicService _gameLogicService;

        #region Init
        public GameLogicController(IGameLogicService gameLogicService)
        {
            _gameLogicService = gameLogicService;
        }

        protected override async Task Initialized()
        {
            await base.Initialized();
            await LoadState();
        }

        protected override async Task Processed()
        {
            await base.Processed();
            await SaveState();
        }
        #endregion

        [BotRoute("/help", UpdateType.Message)]
        public async Task Help()
        {
            await SendTextMessageAsync(
                "Quiz commands list:\n" +
                $"/question - Used to start game or get new question. " +
                $"Can be used with timeot {GameConstants.RequestNewQuestionTimeout.TotalSeconds} sec." +
                "Reply question message to answer on it.\n" +
                "/repeat - To repeat question.\n" +
                "/stats - Show current chat players stats.\n" +
                "To answer on questions you must reply to last question message."
                );
        }

        [BotRoute(UpdateType.Message, Order = 1)]
        public async Task DefaultRepliesListener()
        {
            //Exit if not reply to bot.
            if (Message.ReplyToMessage?.From != BotContext.BotInfo)
                return;

            if (_gameLogicService.IsStateEmpty(State))
            {
                await SendTextMessageAsync("Hi, i am Quiz bot. Use /help to see my commands.");
            }
            else
            {
                (bool isRight, UserStats userStats) = await _gameLogicService
                    .CheckAnswerAndUpdateStats(
                        Message.Text,
                        State,
                        Message.From.Username,
                        Chat.Id.ToString()
                        );

                if (isRight)
                {
                    await SendTextMessageAsync(
                        $"Right!\n@{Message.From.Username} score: {userStats.CorrectAnswersCount}",
                        replyToMessageId: Message.MessageId
                        );
                    await NewQuestion();
                }
                else
                {
                    await SendTextMessageAsync(
                        "Wrong.",
                        replyToMessageId: Message.MessageId
                        );

                }
            }
        }

        [BotRoute("/repeat", UpdateType.Message)]
        public async Task Repeat()
        {
            
            if (_gameLogicService.IsStateEmpty(State))
            {
                await SendTextMessageAsync("Previous game was finished.\nUse /question to start new game.");
            }
            else
            {
                var replyTo = State.QuestionMessageId;
                try
                {
                    //Reply to last question message.
                    await SendTextMessageAsync(
                        "Click to view question.",
                        replyToMessageId: replyTo
                        );
                }
                catch
                {
                    await SendTextMessageAsync("Can't find message to reply.");
                }
            }
        }

        [BotRoute("/stats", UpdateType.Message)]
        public async Task Stats()
        {
            var messengerUsername = Message.From.Username;
            var chatId = Chat.Id.ToString();
            var chatStats = await _gameLogicService.GetChatStats(messengerUsername, chatId);

            var msg = "Stats\n";
            foreach (var user in chatStats.UsersStats)
            {
                int percent = 0;
                if (user.TotalAnswersCount != 0)
                    percent = user.CorrectAnswersCount * 100 / user.TotalAnswersCount;
                msg += $"\n@{user.MessengerUsername}" +
                       $"\n    correct answers: {user.CorrectAnswersCount}" +
                       $"\n    try to answer: {user.TotalAnswersCount}" +
                       $"\n    percentage: {percent}%;";
            }
            await SendTextMessageAsync(msg);
        }

        [BotRoute("/question", UpdateType.Message)]
        public async Task Question()
        {
            if (_gameLogicService.CanAskNewQuestion(State))
            {
                //When game session not exists or can request new question.
                await NewQuestion();
            }
            else
            {
                //When users must wait timeout for new question.
                var timeToNextReques = _gameLogicService.TimeToNextRequest(State);
                await SendTextMessageAsync($"You can request new question after {timeToNextReques.TotalSeconds} seconds.");
            }
        }

        async Task NewQuestion()
        {
            var oldQuestion = State.Question;
            if (oldQuestion != null)
            {
                await SendTextMessageAsync(
                    $"Answer: {oldQuestion.AnswerText}",
                    replyToMessageId: State.QuestionMessageId
                    );
            }

            var newQuestion = await _gameLogicService.NewQuestion();

            var msg = await SendTextMessageAsync($"Question: {newQuestion.QuestionText}.");
            State.Question = newQuestion;
            State.QuestionMessageId = msg.MessageId;
            State.LastQuestionUpdate = DateTime.UtcNow;
            await SaveState();
        }

        #region Game state.
        GameState State { get; set; }

        async Task LoadState()
        {
            //Use only one object to store session. It's not fast (because of serialization), but simple.
            var gameState = await Session.GetOrDefault<GameState>(GameStateKey);
            gameState = gameState ?? new GameState();
            State = gameState;
        }

        async Task SaveState()
        {
            await Session.Set(GameStateKey, State);
        }
        #endregion
    }
}
