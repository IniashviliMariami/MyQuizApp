using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public string CreatorUsername { get; set; }
        public List<Question> Questions { get; set; }
    }
}
