using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types.Enums;
using TelegаQuiz.DAL;
using TelegаQuiz.Entities;

namespace TelegаQuiz.TelegramControllers
{
    public class GameController : BotController
    {
        const string GameSessionKey = "GameSession";

        readonly IDatabaseService _db;

        //public GameController(IDatabaseService db)
        //{
        //    _db = db;
        //}

        public GameController()
        {
        }

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

            var gameSession = await Session.GetOrDefault<GameSession>(GameSessionKey);
            if (gameSession == null)
            {
                await SendTextMessageAsync("Hi, i am Quiz bot. Use /help to see my commands.");
            }
            else
            {
                //I am too lazy to optimize this. Just check substring in answer.
                var answerIsRight = Message.Text.Trim().ToLower().Contains(
                    gameSession.Question.AnswerText.ToLower().Trim()
                    );
                if (answerIsRight)
                {
                    //TODO Will integrate later...
                    var chatStats = new ChatStats();
                    var currentUserStats = new UserStats();
                    //...

                    await SendTextMessageAsync(
                        $"Right!\n@{Message.From.Username} score: {currentUserStats.CorrectAnswersCount}",
                        replyToMessageId: Message.From.Id
                        );

                    //TODO Save new stats in database.
                    await NewQuestion(gameSession);
                }
                else
                {
                    await SendTextMessageAsync(
                        "Wrong.",
                        replyToMessageId: Message.From.Id
                        );

                    //TODO Update stats in database.
                }
            }
        }

        [BotRoute("/repeat", UpdateType.Message)]
        public async Task Repeat()
        {
            //Use only one object to store session. It's not fast (because of serialization), but simple.
            var gameSession = await Session.GetOrDefault<GameSession>(GameSessionKey);
            if (gameSession == null)
            {
                await SendTextMessageAsync("Previous game was finished.\nUse /question to start new game.");
            }
            else
            {
                var replyTo = gameSession.QuestionMessageId;
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
            //TODO Will integrate later...
            var chatStats = new ChatStats();
            //...

            var msg = "Stats\n";
            foreach (var user in chatStats.UsersStats)
            {
                int percent = user.CorrectAnswersCount * 100 / user.TotalAnswersCount;
                msg += $"\n@{user.Username}:" +
                       $"\n  correct answers: {user.CorrectAnswersCount}" +
                       $"\n  try to answer: {user.CorrectAnswersCount}" +
                       $"\n  percentage: {percent}%;";
            }
            await SendTextMessageAsync(msg);
        }

        [BotRoute("/question", UpdateType.Message)]
        public async Task Question()
        {
            var gameSession = await Session.GetOrDefault<GameSession>(GameSessionKey);
            //If game session not exists or can request new question.
            if (gameSession == null || DateTime.UtcNow - gameSession.LastQuestionUpdate > GameConstants.RequestNewQuestionTimeout)
            {
                await NewQuestion(gameSession);
            }
            else
            {
                var timeToNextReques = GameConstants.RequestNewQuestionTimeout - (DateTime.UtcNow - gameSession.LastQuestionUpdate);
                await SendTextMessageAsync($"You can request new question after {timeToNextReques.TotalSeconds} seconds.");
            }
        }

        async Task NewQuestion(GameSession gameSession = null)
        {
            gameSession = gameSession ?? new GameSession();

            var oldQuestion = gameSession.Question;
            if (oldQuestion != null)
            {
                await SendTextMessageAsync(
                    $"Answer: {oldQuestion.AnswerText}",
                    replyToMessageId: gameSession.QuestionMessageId
                    );
            }

            //TODO Here we read it from db and else...
            var newQuestion = new Question();
            //...

            var msg = await SendTextMessageAsync($"Question: {newQuestion.QuestionText}.");
            gameSession.Question = newQuestion;
            gameSession.QuestionMessageId = msg.MessageId;
            gameSession.LastQuestionUpdate = DateTime.UtcNow;
            await Session.Set(GameSessionKey, gameSession);
        }

    }
}
