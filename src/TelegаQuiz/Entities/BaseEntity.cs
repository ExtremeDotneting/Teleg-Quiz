using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace TelegаQuiz.Entities
{
    public class BaseEntity<TKey>
    {
        [BsonId(true)]
        public TKey Id { get; set; }
    }

    public class BaseEntity:BaseEntity<int>
    {
    }
}
