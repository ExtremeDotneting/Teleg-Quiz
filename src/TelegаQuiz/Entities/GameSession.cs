using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegаQuiz.Entities
{
    /// <summary>
    /// Used in session storage only.
    /// </summary>
    public class GameSession
    {
        public int QuestionMessageId { get; set; }

        public Question Question { get; set; }

        public DateTime LastQuestionUpdate { get; set; }


    }
}
