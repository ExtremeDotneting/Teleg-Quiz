using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegаQuiz.Entities
{
    public class Question:BaseEntity
    {
        public string QuestionText { get; set; }

        public string AnswerText { get; set; }
    }
}
