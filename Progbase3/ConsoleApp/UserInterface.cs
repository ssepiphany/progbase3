using Microsoft.Data.Sqlite;
using System.Collections.Generic;

static class UserInterface
{
    public static void ProcessGenerate(string[] command, SqliteConnection connection)
    {
        Validator.ValidateLength(command.Length, 3);
        string entity = command[1];
        Validator.ValidateEntity(entity);
        int num = Validator.ValidateNumber(command[2]);
        MovieRepository movieRepo = new MovieRepository(connection);
        ActorRepository actorRepo = new ActorRepository(connection);
        ReviewRepository reviewRepo = new ReviewRepository(connection);
        UserRepository userRepo = new UserRepository(connection);
        switch(entity)
        {
            case "movie": 
                List<Movie> list1 = Generator.GenerateMovies(num);
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
                List<Actor> list2 = Generator.GenerateActors(num);
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
                List<Review> list3 = Generator.GenerateReviews(num, userRepo, movieRepo);
                for(int i = 0; i < num; i++)
                {
                    reviewRepo.Insert(list3[i]);
                }
                break;
            case "user":
                List<User> list4 = Generator.GenerateUsers(num);
                for(int i = 0; i < num; i++)
                {
                    userRepo.Insert(list4[i]);
                }
                break;
        }
    }
}