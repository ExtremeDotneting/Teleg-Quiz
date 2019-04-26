using System.Collections.Generic;

namespace TelegаQuiz.Entities
{
    public class ChatStats:BaseEntity
    {
        public long ChatId { get; set; }

        /// <summary>
        /// In nosql will work not too fast if there many stats in chat, because will be loaded every 
        /// </summary>
        public ICollection<UserStats> UsersStats { get; set; }
    }
}
