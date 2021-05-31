using System;
using System.Globalization;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace GeneratorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SetDotSeparator();
            string dbPath = "C:/Users/Sofia/projects/progbase3/data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            MovieRepository movieRepo = new MovieRepository(connection);
            ActorRepository actorRepo = new ActorRepository(connection);
            ReviewRepository reviewRepo = new ReviewRepository(connection);
            UserRepository userRepo = new UserRepository(connection);

            Console.WriteLine("Please, enter your command:");
            string[] command = Console.ReadLine().Trim().Split();
            if(command[0] == "generate")
            {
                // generate {movie} {5}
                Validator.ValidateLength(command.Length, 3);
                string entity = command[1];
                Validator.ValidateEntity(entity);
                int num = Validator.ValidateNumber(command[2]);
                HashSet<string> logins = userRepo.GetAllLogins();
                switch(entity)
                {
                    case "movie": 
                        List<Movie> list1 = Generator.GenerateMovies(num, movieRepo);
                        for(int i = 0; i < num; i++)
                        {
                            movieRepo.Insert(list1[i]);
                        }
                        if(actorRepo.GetCount() != 0) 
                        {
                            Dictionary<int, List<int>> connection1 = new Dictionary<int, List<int>>();
                            for(int i = 0; i < num*2; i++)
                            {
                                Generator.CreateActorsMoviesConnection(connection1, movieRepo, actorRepo.GetAll());
                            }
                        }
                        break;
                    case "actor":
                        List<Actor> list2 = Generator.GenerateActors(num, actorRepo);
                        for(int i = 0; i < num; i++)
                        {
                            actorRepo.Insert(list2[i]);
                        }
                        if(movieRepo.GetCount() != 0)
                        {
                            Dictionary<int, List<int>> connection2 = new Dictionary<int, List<int>>();
                            for(int i = 0; i < num*2; i++)
                            {
                                Generator.CreateActorsMoviesConnection(connection2, movieRepo, list2);
                            }
                        } 
                        break;
                    case "review":
                        List<Review> list3 = Generator.GenerateReviews(num, userRepo, movieRepo, logins);
                        for(int i = 0; i < num; i++)
                        {
                            reviewRepo.Insert(list3[i]);
                        }
                        break;
                    case "user":
                        List<User> list4 = Generator.GenerateUsers(num, logins);
                        for(int i = 0; i < num; i++)
                        {
                            userRepo.Insert(list4[i]);
                        }
                        break;
                }
            }
            connection.Close();
        }

        static void SetDotSeparator()
        {
            CultureInfo englishUSCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = englishUSCulture;
        }
    }
}
