using System.Collections.Generic;
using System;
using System.IO ;
using System.Text;
using System.Security.Cryptography;

static class Generator
{
    public static List<Actor> GenerateActors(int num, ActorRepository actorRepo)
    {
        Random random = new Random();
        string[] genders = new string[] {"male", "female"};
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
            if(actorRepo.GetByFullname(values[1]) != null)
            {
                i--;
                continue;
            }
            actor.fullname = values[1] ;  
            actor.age = random.Next(15, 81);
            actor.gender = genders[random.Next(0,2)];
            actors.Add(actor) ; 
        }
        sr.Close() ; 
        return actors ; 
    }

    public static List<Movie> GenerateMovies(int num, MovieRepository movieRepository)
    {
        Random random = new Random();
        string[] genres = new string[] {"comedy", "action", "drama", "horror", "fantasy", "other"};
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
            if(movieRepository.GetByTitle(values[1]) != null)
            {
                i--;
                continue;
            }
            movie.title = values[1];  
            movie.genre = genres[random.Next(0,genres.Length)];
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

    public static List<Review> GenerateReviews(int num, UserRepository userRepo, MovieRepository movieRepo, HashSet<string> logins)
    {
        List<Review> reviews = new List<Review>();
        Dictionary<int, List<int>> connection = new Dictionary<int, List<int>>();
        if(userRepo.GetCount() == 0) GenerateUsers(num/2, logins);
        if(movieRepo.GetCount() == 0) GenerateMovies(num/2, movieRepo);
        RandomDateTime date = new RandomDateTime();
        Random random = new Random();
        for(int i = 0; i < num; i++)
        {
            Review review = new Review();
            review.value = random.Next(1,11);
            review.createdAt = date.Next();
            review.imported = false;
            CreateConnectionInReviews(ref review, userRepo.GetCount(), movieRepo.GetCount(), connection);
            reviews.Add(review);
        }
        return reviews;
    }

    public static List<User> GenerateUsers(int num, HashSet<string> logins)
    {
        string filePath = "C:/Users/Sofia/projects/progbase3/data/generator/users.csv";
        Random random = new Random();
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
            if (logins.Contains(values[2]))
            {
                i--;
                continue;
            }

            user.login = values[2];
            user.fullname = values[1];
            user.createdAt = DateTime.Parse(values[0]);
            user.moderator = random.Next(1,100) > 90;

            string password = CreatePassword(random.Next(5, 15));

            SHA256 sha256 = SHA256.Create();
            user.password =  GetHash(sha256, password);
            sha256.Dispose();
            users.Add(user);
        }
        sr.Close() ; 
        return users;
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
       byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
 
       StringBuilder sb = new StringBuilder();

       for (int i = 0; i < data.Length; i++)
       {
            sb.Append(data[i].ToString("x2"));
       }
 
       return sb.ToString();
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
        moviesRepo.ConnectMovieActor(movieId, actorId);
    }

    private static string CreatePassword(int length)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder sb = new StringBuilder();
        Random random = new Random();
        while (0 < length--)
        {
            sb.Append(chars[random.Next(chars.Length)]);
        }
        return sb.ToString();
    }

    class RandomDateTime
    {
        DateTime start;
        Random gen;
        int range;

        public RandomDateTime()
        {
            start = new DateTime(1995, 1, 1);
            gen = new Random();
            range = (DateTime.Today - start).Days;
        }

        public DateTime Next()
        {
            return start.AddDays(gen.Next(range)).AddHours(gen.Next(0,24)).AddMinutes(gen.Next(0,60)).AddSeconds(gen.Next(0,60));
        }
    }
}