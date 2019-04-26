using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegаQuiz
{
    public static class GameConstants
    {
        public static TimeSpan RequestNewQuestionTimeout { get; } = TimeSpan.FromSeconds(60);
    }
}
