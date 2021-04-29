using Microsoft.Data.Sqlite;
using System.Collections.Generic;

static class UserInterface
{
    public static void ProcessGenerate(string[] command, SqliteConnection connection)
    {
        string entity = command[1];
        Validator.ValidateEntity(entity);
        int num;
        if(!int.TryParse(command[2], out num))
        {
            throw new System.Exception($"Invalid number: {command[2]}.");
        }
        switch(entity)
        {
            case "movie": 
                List<Movie> list1 = Generator.GenerateMovies(num);
                MovieRepository movieRepo = new MovieRepository(connection);
                System.Console.WriteLine($"Lneght : {list1.Count}.");
                for(int i = 0; i < num; i++)
                {
                    movieRepo.Insert(list1[i]);
                }
                break;
            case "actor":
                List<Actor> list2 = Generator.GenerateActors(num);
                ActorRepository actorRepo = new ActorRepository(connection);
                for(int i = 0; i < num; i++)
                {
                    actorRepo.Insert(list2[i]);
                }
                break;
            case "review":
                List<Review> list3 = Generator.GenerateReviews(num);
                ReviewRepository reviewRepo = new ReviewRepository(connection);
                for(int i = 0; i < num; i++)
                {
                    reviewRepo.Insert(list3[i]);
                }
                break;
            // case "user":
            //     Generator.GenerateUsers(num);
            //     break;
        }
    }
}