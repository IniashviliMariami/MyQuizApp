using QuizData;
using QuizModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizRepository
{
    public class QuizzesRepository
    {
        private readonly DataAccess<Quiz> _dataAccess;
        private List<Quiz> _quizList;

        public QuizzesRepository(string filepath)
        {
            _dataAccess = new DataAccess<Quiz>(filepath);
            _quizList = _dataAccess.LoadData();
        }

        public Quiz CreatQuiz(User user, List<Question> question)
        {
            if (question.Count != 5)
            {
                throw new ArgumentException("The quiz should have 5 questions.");
            }
            var quiz = new Quiz
            {
               
                QuizId = _quizList.Any() ? _quizList.Max(q => q.QuizId) + 1 : 1,
                CreatorUsername = user.Username,
                Questions = question
            };
            _quizList.Add(quiz);
            SaveChenge();
            return quiz;
        }

        private void SaveChenge()
        {
            _dataAccess.SaveData(_quizList);
        }

        public void DeleteQuiz(User user, int quizId)
        {
            var quiz=_quizList.FirstOrDefault(q=>q.QuizId == quizId);
            if (quiz==null)
            {
                throw new Exception("You can only delete your own quizzes.");
            }
            _quizList.Remove(quiz);
            SaveChenge();
        }
        public List<Quiz> GetAvaibleQuiz(User user)
        {
            return _quizList.Where(q=>q.CreatorUsername != user.Username).ToList();
        }

        public List<Quiz> GetUsersQuiz(User user)
        {
            return _quizList.Where(q => q.CreatorUsername == user.Username).ToList();
        }
        public void EditQuiz(User currentUser, int quizId, List<Question> newQuestions)
        {
            var quiz = _quizList.FirstOrDefault(q => q.QuizId == quizId);
            if (quiz == null) throw new Exception("Quiz not found.");
            if (quiz.CreatorUsername != currentUser.Username) throw new Exception("You can only edit your own quizzes.");

            quiz.Questions = newQuestions;
            SaveChenge();
        }

        public Quiz GetQuizById(int quizId)
        {
            return _quizList.FirstOrDefault(q => q.QuizId == quizId);
        }

    }
}
