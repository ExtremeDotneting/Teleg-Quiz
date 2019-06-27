using System;
using System.Linq;
using System.Threading.Tasks;

namespace TelegаQuiz.Entities
{
    /// <summary>
    /// Stored in <see cref="ChatStats"/> field.
    /// </summary>
    public class UserStats
    {
        /// <summary>
        /// Id of <see cref="User"/>.
        /// </summary>
        public string MessengerUsername { get; set; }

        public int CorrectAnswersCount { get; set; }

        public int TotalAnswersCount { get; set; }
    }
}
