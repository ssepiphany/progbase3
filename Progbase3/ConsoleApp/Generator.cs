using System.Collections.Generic;
using System;
using System.IO ; 

static class Generator
{
    public static List<Actor> GenerateActors(int num)
    {
        string filePath = "C:/Users/Sofia/projects/progbase3/data/generator/IMDB names.csv";
        List<Actor> actors = new List<Actor>(); 
        StreamReader sr = new StreamReader(filePath) ; 
        int counter = 0;     
        for(int i = 0; i < num; i++)
        {
            string row = sr.ReadLine() ;
            if ( row == null ) break ;
            if(counter == 0) 
            {
                i--;
                counter++;
                continue;
            }
            string[] values = row.Split(',') ;
            if(values.Length != 17)
            {
                i--;
                continue;
            }
            Actor actor = new Actor(); 
            actor.fullname = values[1] ;  
            actors.Add(actor) ; 
        }
        sr.Close() ; 
        return actors ; 
    }

    public static List<Movie> GenerateMovies(int num)
    {
        string filePath = "C:/Users/Sofia/projects/progbase3/data/generator/IMDB movies.csv";
        List<Movie> movies = new List<Movie>();
        StreamReader sr = new StreamReader(filePath) ; 
        int counter = 0;       
        for(int i = 0; i < num; i++)
        {
            string row = sr.ReadLine() ;
            if ( row == null ) break ;
            if(counter == 0) 
            {
                i--;
                counter++;
                continue;
            }
            string[] values = row.Split(',') ;
            if(values.Length != 22)
            {
                i--;
                continue;
            }
            Movie movie = new Movie(); 
            movie.title = values[1];  
            if(!DateTime.TryParse(values[4], out movie.releaseDate))
            {
                i--;
                continue;
            }
            movies.Add(movie); 
        }
        sr.Close() ; 
        return movies ; 
    }

    public static List<Review> GenerateReviews(int num, UserRepository userRepo, MovieRepository movieRepo)
    {
        List<Review> reviews = new List<Review>();
        Dictionary<int, List<int>> connection = new Dictionary<int, List<int>>();
        if(userRepo.GetCount() == 0) GenerateUsers(num/2);
        if(movieRepo.GetCount() == 0) GenerateMovies(num/2);
        Random random = new Random();
        for(int i = 0; i < num; i++)
        {
            Review review = new Review();
            review.value = Math.Round(random.Next(1,10) + random.NextDouble(),1);
            CreateConnectionInReviews(ref review, userRepo.GetCount(), movieRepo.GetCount(), connection);
            reviews.Add(review);
        }
        return reviews;
    }

    public static List<User> GenerateUsers(int num)
    {
        string filePath = "C:/Users/Sofia/projects/progbase3/data/generator/users.csv";
        StreamReader sr = new StreamReader(filePath) ; 
        int counter = 0;
        List<User> users = new List<User>();
        for(int i = 0; i < num; i++)
        {
            string row = sr.ReadLine() ;
            if ( row == null ) break ;
            if(counter == 0) 
            {
                i--;
                counter++;
                continue;
            }
            string[] values = row.Split(',') ;
            User user = new User();
            user.login = values[2];
            user.fullname = values[1];
            user.createdAt = DateTime.Parse(values[0]);
            users.Add(user);
        }
        sr.Close() ; 
        return users;
    }

    private static void CreateConnectionInReviews(ref Review review, long userCount, long movieCount, Dictionary<int, List<int>> connection)
    {
        Random random = new Random();
        review.movieId = random.Next(1, (int)movieCount+1);
        review.userId = random.Next(1, (int)userCount+1);
        List<int> value;
        if(!connection.TryGetValue(review.movieId, out value))
        {
            List<int> list = new List<int>();
            list.Add(review.userId);
            connection.Add(review.movieId, list);
        }
        else
        {
            if(!value.Contains(review.userId))
            {
                value.Add(review.userId);
            }
            else
            {
                CreateConnectionInReviews(ref review, userCount, movieCount, connection);
            }
        }
    }

    public static void CreateActorsMoviesConnection(Dictionary<int, List<int>> connection, MovieRepository moviesRepo, List<Actor> actorsList)
    {
        Random random = new Random();
        int movieId = random.Next(1, (int)moviesRepo.GetAll().Count+1);
        int actorId = random.Next(1, (int)actorsList.Count+1);
        List<int> value;
        if(!connection.TryGetValue(movieId, out value))
        {
            List<int> list = new List<int>();
            list.Add(actorId);
            connection.Add(movieId, list);
        }
        else
        {
            if(!value.Contains(actorId))
            {
                value.Add(actorId);
            }
            else
            {
                CreateActorsMoviesConnection(connection, moviesRepo, actorsList);
            }
        }
        moviesRepo.ConnectWithActors(movieId, actorId);
    }
}