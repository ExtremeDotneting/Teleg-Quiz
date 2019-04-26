using System;
using System.Linq;
using System.Threading.Tasks;

namespace TelegаQuiz.Entities
{
    public class UserStats
    {
        public string Username { get; set; }

        public int CorrectAnswersCount { get; set; }

        public int TotalAnswersCount { get; set; }
    }
}
