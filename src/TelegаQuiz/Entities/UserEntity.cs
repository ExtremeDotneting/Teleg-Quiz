using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegаQuiz.Entities
{
    public class UserEntity : BaseEntity
    {
        public string MessengerUsername { get; set; }

        public AccessLevel AccessLevel { get; set; }
    }
}
