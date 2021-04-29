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
            actor.id = i+1; 
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
            movie.id = i+1; 
            movies.Add(movie); 
        }
        sr.Close() ; 
        return movies ; 
    }

    public static List<Review> GenerateReviews(int num)
    {
        List<Review> reviews = new List<Review>();
        Random random = new Random();
        for(int i = 0; i < num; i++)
        {
            Review review = new Review();
            review.id = i+1;
            review.value = Math.Round(random.Next(1,10) + random.NextDouble(),1);
            reviews.Add(review);
        }
        return reviews;
    }

    // public static void GenerateUsers(int num)
    // {
        
    // }
}