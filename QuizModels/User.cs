using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int HighScore { get; set; } = 0;
        public List<int> CreatedQuizIds { get; set; } = new List<int>();
    }
}
