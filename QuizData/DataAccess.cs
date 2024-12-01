using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizData
{
    public class DataAccess<T>
    {
        private readonly string _filePath;

        public DataAccess(string filePath)
        {
            _filePath = filePath;
        }
        public List<T> LoadData()
        {
            if (!File.Exists(_filePath)) return new List<T>();
            var jsonData = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<T>>(jsonData) ?? new List<T>();
        }
        public void SaveData(List<T> data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, jsonData);
        }
    }
}
