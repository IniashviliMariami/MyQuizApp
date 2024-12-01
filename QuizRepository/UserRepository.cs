using QuizData;
using QuizModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizRepository
{
    public class UserRepository
    {
        private readonly DataAccess<User> _dataAccess;
        private List<User> _users;

        public UserRepository(string filePath)
        {
            _dataAccess = new DataAccess<User>(filePath);
            _users = _dataAccess.LoadData();
        }

        public User Register(string username, string password)
        {
            if(_users.Any(u=>u.Username == username))
            {
                throw new Exception("Username exist.");
            }
            var user=new User
            {
                Username=username,
                Password=password
            };
            _users.Add(user);
            saveChange(user);
            return user;
        }

        private void saveChange(User user)
        {
            _dataAccess.SaveData(_users);
        }

        public User Login(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password)
                ?? throw new Exception("Invalid username or password");
        }

        public List<User> TopUsers()
        {
            return _users.OrderByDescending(u=>u.HighScore).Take(10).ToList();
        }
        public void UpdateHightScore(string username,int newHighScore)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.HighScore = newHighScore;
                saveChange(user);
            }
        }
    }
}
