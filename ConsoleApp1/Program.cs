using QuizModels;
using QuizRepository;
using System;
using System.Diagnostics;


namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var quizRepository = new QuizzesRepository("C:\\Users\\marii\\OneDrive\\Desktop\\MyQuizApp\\ConsoleApp1\\quizzes.json");
            var userRepository = new UserRepository("C:\\Users\\marii\\OneDrive\\Desktop\\MyQuizApp\\ConsoleApp1\\users.json");
           
           Console.WriteLine("   start Quiz App");

            User currentUser = null;
            while (true)
            {
                try
                {
                    if (currentUser == null)
                    {
                        Console.WriteLine("\n1. Register\n2. Login\n3. Exit");
                        Console.Write("Enter the number: ");
                        var choice = Console.ReadLine();

                        if (choice == "1")
                        {
                            Console.Write("Enter username: ");
                            var username = Console.ReadLine();
                            Console.Write("Enter password: ");
                            var password = Console.ReadLine();
                            currentUser = userRepository.Register(username, password);
                            Console.WriteLine("Registration successful!");
                        }
                        else if (choice == "2")
                        {
                            Console.Write("Enter username: ");
                            var username = Console.ReadLine();
                            Console.Write("Enter password: ");
                            var password = Console.ReadLine();
                            currentUser = userRepository.Login(username, password);
                            Console.WriteLine($"Welcome back, {currentUser.Username}!");
                        }
                        else if (choice == "3")
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n   Main menu");

                        Console.WriteLine("1. Create Quiz\n2. Game Quiz\n3. View High Scores\n4. Edit Quiz\n5. Delete Quiz\n6. Logout");
                        Console.Write("Enter the number: ");
                        var choice = Console.ReadLine();

                        if (choice == "1")
                        {
                            var questions = new List<Question>();

                            for (int i = 0; i < 5; i++)
                            {
                                string title;
                                do
                                {
                                    Console.Write($"Enter question {i + 1}: ");
                                    title = Console.ReadLine();
                                    if (string.IsNullOrWhiteSpace(title))
                                        Console.WriteLine("Question title cannot be empty.");
                                } while (string.IsNullOrWhiteSpace(title));

                                List<string> options;
                                do
                                {
                                    Console.WriteLine("Enter 4 options (use commas to separate): ");
                                    options = Console.ReadLine()?.Split(',').Select(o => o.Trim()).ToList();
                                    if (options == null || options.Count != 4)
                                        Console.WriteLine("You must provide exactly 4 options.");
                                } while (options == null || options.Count != 4);

                                int correctIndex;
                                do
                                {
                                    Console.Write("Enter the correct option (1-4): ");
                                } while (!int.TryParse(Console.ReadLine(), out correctIndex) || correctIndex < 1 || correctIndex > 4);

                                questions.Add(new Question
                                {
                                    Title = title,
                                    Options = options,
                                    CorrectOptionIndex = correctIndex - 1
                                });

                                Console.WriteLine($"Question {i + 1} added successfully!\n");
                            }
                            quizRepository.CreatQuiz(currentUser, questions);
                            Console.WriteLine("Quiz created successfully!");
                            while (true)
                            {
                                Console.Write("Return to main menu?( Enter number '1'): ");


                                var choices = Console.ReadLine();

                                if (choices == "1")
                                {
                                    break;

                                }
                            }

                        }
                        else if (choice == "2")
                        {
                            bool gameRunning = true;

                            while (gameRunning)
                            {
                                var availableQuizzes = quizRepository.GetAvaibleQuiz(currentUser);
                                if (!availableQuizzes.Any())
                                {
                                    Console.WriteLine("No quizzes available.");
                                    break;
                                }

                                Console.WriteLine("Available quizzes:");
                                for (int i = 0; i < availableQuizzes.Count; i++)
                                {
                                    Console.WriteLine($"{i + 1}) Quiz {availableQuizzes[i].QuizId} by {availableQuizzes[i].CreatorUsername}");
                                }

                                Console.Write("Choose a quiz: ");
                                int quizChoice;
                                while (!int.TryParse(Console.ReadLine(), out quizChoice) || quizChoice < 1 || quizChoice > availableQuizzes.Count)
                                {
                                    Console.WriteLine("Invalid input.");
                                }
                                var selectedQuiz = availableQuizzes[quizChoice - 1];

                                bool quizCompleted = false;
                                while (!quizCompleted)
                                {
                                    int score = 0;
                                    var stopwatch = new Stopwatch();
                                    stopwatch.Start();

                                    var cts = new CancellationTokenSource();
                                    var timerTask = Task.Run(() =>
                                    {
                                        while (!cts.Token.IsCancellationRequested)
                                        {
                                            if (stopwatch.Elapsed.TotalMinutes >= 0.5)
                                            {
                                                Console.WriteLine("\nTime is up!");
                                                Console.Write("Please press Enter _ ");
                                                cts.Cancel();
                                            }
                                        }
                                    }, cts.Token);

                                   
                                    foreach (var question in selectedQuiz.Questions)
                                    {
                                        if (cts.Token.IsCancellationRequested)
                                            break;

                                        Console.WriteLine(question.Title);
                                        for (int i = 0; i < question.Options.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. {question.Options[i]}");
                                        }

                                        int answer = 0;
                                        while (true)
                                        {
                                            Console.Write("Your answer: ");
                                            var input = Console.ReadLine();

                                            if (cts.Token.IsCancellationRequested) 
                                                break;

                                            if (int.TryParse(input, out answer) && answer >= 1 && answer <= question.Options.Count)
                                            {
                                                break; 
                                            }
                                            Console.WriteLine("Invalid input. Please enter a valid option number.");
                                        }

                                        if (cts.Token.IsCancellationRequested)
                                            break;

                                        if (answer - 1 == question.CorrectOptionIndex)
                                        {
                                            score += 20;
                                        }
                                        else
                                        {
                                            score -= 20;
                                        }
                                    }

                                    stopwatch.Stop();
                                    cts.Cancel();

                                   
                                    if (stopwatch.Elapsed.TotalMinutes >= 0.5)
                                    {
                                        Console.WriteLine("You failed to complete the quiz within the time limit.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Your score: {score}");
                                        if (score > currentUser.HighScore)
                                        {
                                            currentUser.HighScore = score;
                                            Console.WriteLine("New high score!");
                                            userRepository.UpdateHightScore(currentUser.Username, score);
                                        }
                                    }

                                    
                                    Console.WriteLine("\nWhat would you like to do next?");
                                    Console.WriteLine("1) Retry this quiz");
                                    Console.WriteLine("2) Choose another quiz");
                                    Console.WriteLine("3) Return to main menu");
                                    Console.Write("Enter the number: ");

                                    int postQuizChoice;
                                    while (!int.TryParse(Console.ReadLine(), out postQuizChoice) || postQuizChoice < 1 || postQuizChoice > 3)
                                    {
                                        Console.WriteLine("Invalid input. Please choose 1, 2, or 3.");
                                    }

                                    switch (postQuizChoice)
                                    {
                                        case 1:
                                            break; 
                                        case 2:
                                            quizCompleted = true; 
                                            break;
                                        case 3:
                                            gameRunning = false; 
                                            quizCompleted = true; 
                                            break;
                                    }
                                }
                            }
                            
                        }
                        else if (choice == "3")
                        {
                            Console.WriteLine();
                            var topUsers = userRepository.TopUsers();

                            if (!topUsers.Any())
                            {
                                Console.WriteLine("No users available.");
                            }
                            else
                            {
                                Console.WriteLine("   Top 10 Users:");
                                foreach (var user in topUsers)
                                {
                                    Console.WriteLine($"{user.Username}: {user.HighScore}");
                                }
                            }
                            while (true)
                            {
                                Console.Write("Return to main menu?( Enter number '1'): ");
                               

                                var choices = Console.ReadLine();

                                if (choices == "1")
                                {
                                    break; 

                                }
                            }
                        }
                        else if (choice == "4")
                        {

                            while (true)
                            {
                                var userQuizzes = quizRepository.GetUsersQuiz(currentUser);

                                if (!userQuizzes.Any())
                                {
                                    Console.WriteLine("You don't have any quizzes to edit.");
                                    break;
                                }

                                Console.WriteLine("Your quizzes:");
                                for (int i = 0; i < userQuizzes.Count; i++)
                                {
                                    Console.WriteLine($"{i + 1}. Quiz ID: {userQuizzes[i].QuizId}");
                                }

                                Console.Write("Choose a quiz number  (or 0 to cancel): ");
                                if (!int.TryParse(Console.ReadLine(), out int quizChoice) || quizChoice < 0 || quizChoice > userQuizzes.Count)
                                {
                                    Console.WriteLine("Invalid input. Try again.");
                                    continue;
                                }

                                if (quizChoice == 0)
                                {
                                    Console.WriteLine("Edit operation cancelled.");
                                    break;
                                }

                                var selectedQuiz = userQuizzes[quizChoice - 1];
                                var questions = new List<Question>();

                                for (int i = 0; i < 5; i++)
                                {
                                    string title;
                                    while (true)
                                    {
                                        Console.Write($"Enter question {i + 1}: ");
                                        title = Console.ReadLine().Trim();

                                        if (!string.IsNullOrEmpty(title))
                                            break;

                                        Console.WriteLine("Question title cannot be empty. Please try again.");
                                    }

                                    string[] options;
                                    while (true)
                                    {
                                        Console.WriteLine("Enter 4 options (separated by commas): ");
                                        options = Console.ReadLine()?.Split(',');

                                        if (options != null && options.Length == 4)
                                            break;

                                        Console.WriteLine("Please enter exactly 4 options separated by commas.");
                                    }

                                    int correctIndex;
                                    while (true)
                                    {
                                        Console.Write("Enter the correct option (1-4): ");
                                        if (int.TryParse(Console.ReadLine(), out correctIndex) && correctIndex >= 1 && correctIndex <= 4)
                                            break;

                                        Console.WriteLine("Invalid option number. Please enter a number between 1 and 4.");
                                    }

                                    questions.Add(new Question
                                    {
                                        Title = title,
                                        Options = new List<string>(options),
                                        CorrectOptionIndex = correctIndex - 1
                                    });
                                }
                                quizRepository.EditQuiz(currentUser, selectedQuiz.QuizId, questions);
                                Console.WriteLine("Quiz edited successfully!");
                                break;

                            }

                        }

                        else if (choice == "5")
                        {
                            var userQuizzes = quizRepository.GetUsersQuiz(currentUser);

                            if (!userQuizzes.Any())
                            {
                                Console.WriteLine("You don't have any quizzes to delete.");
                            }
                            else
                            {
                                Console.WriteLine("Your quizzes:");
                                for (int i = 0; i < userQuizzes.Count; i++)
                                {
                                    Console.WriteLine($"{i + 1}. Quiz ID: {userQuizzes[i].QuizId}");
                                }

                                Console.Write("Choose a quiz number  (or 0 to cancel): ");

                                if (!int.TryParse(Console.ReadLine(), out int UserChoice) || UserChoice < 0 || UserChoice > userQuizzes.Count)
                                {
                                    Console.WriteLine("Invalid input.");
                                }
                                else if (UserChoice == 0)
                                {
                                    Console.WriteLine("Deletion cancelled.");
                                }
                                else
                                {
                                    var selectedQuiz = userQuizzes[UserChoice - 1];

                                    quizRepository.DeleteQuiz(currentUser, selectedQuiz.QuizId);
                                    Console.WriteLine($"Quiz with ID {selectedQuiz.QuizId} has been deleted successfully.");

                                }
                            }
                            while (true)
                            {
                                Console.Write("Return to main menu?( Enter number '1'): ");


                                var choices = Console.ReadLine();

                                if (choices == "1")
                                {
                                    break;
                                }
                            }
                        }
                        else if (choice == "6")
                        {
                            currentUser = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

    }
}
