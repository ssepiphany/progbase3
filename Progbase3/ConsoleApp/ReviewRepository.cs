using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;

public class ReviewRepository
{
    private SqliteConnection connection;

    public ReviewRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public Review GetById(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader(); 
        if ( reader.Read())
        {
            Review review = ReadReview(reader); 
            reader.Close(); 
            return review;
        } 
        else
        {
            reader.Close(); 
            return null; 
        } 
    }

    public List<Review> GetPage(int pageNum, int pageLength)
    {
        if(pageNum < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Review> page = new List<Review>();
        SqliteCommand command = connection.CreateCommand() ;
        command.CommandText = @"SELECT * FROM reviews CROSS JOIN movies 
            WHERE movies.id = reviews.movieId LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Review review = FillReviewWithMovies(reader); 
            page.Add(review); 
        }
        reader.Close() ; 
        return page ; 
    }

    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (float)pageLength) ; 
    }

    public List<Review> GetAll()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews"; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader) ;  
            list.Add(review); 
        }
        reader.Close() ; 
        return list ;  
    }

    public List<Review> GetAllByAuthorId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE userId = $id"; 
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader);  
            list.Add(review); 
        }
        reader.Close() ; 
        return list ;  
    }

    public List<Review> GetAllByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE movieId = $id"; 
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader);  
            list.Add(review); 
        }
        reader.Close() ; 
        return list ;  
    }

    public bool DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1 ; 
    }

    public int DeleteAllByAuthorId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE userId = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int DeleteAllByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE movieId = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }


    public int Insert(Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO reviews(value, movieId, userId, createdAt, imported)
            VALUES($value, $movieId, $userId, $createdAt, $imported); 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$value" , review.value ) ; 
        command.Parameters.AddWithValue("$movieId", review.movieId); 
        command.Parameters.AddWithValue("$userId", review.userId) ; 
        command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o")) ;
        command.Parameters.AddWithValue("$imported", review.imported.ToString()) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE reviews SET value = $value, movieId = $movieId, userId = $userId, createdAt = $createdAt, imported = $imported WHERE id = $id" ; 
        command.Parameters.AddWithValue("$value", review.value); 
        command.Parameters.AddWithValue("$id", review.id) ; 
        command.Parameters.AddWithValue("$movieId", review.movieId); 
        command.Parameters.AddWithValue("$userId", review.userId) ; 
        command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o")) ; 
        command.Parameters.AddWithValue("$imported", review.imported.ToString()) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    private Review ReadReview(SqliteDataReader reader)
    {
        Review review =  new Review(); 
        review.id = int.Parse(reader.GetString(0)) ; 
        review.value = int.Parse(reader.GetString(1)); 
        review.movieId = int.Parse(reader.GetString(2)) ; 
        review.userId = int.Parse(reader.GetString(3)) ;
        review.createdAt = DateTime.Parse(reader.GetString(4)); 
        review.imported = bool.Parse(reader.GetString(5));
        return review ;
    }

    public long GetCount()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM reviews"; 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public User GetReviewsForExport(User user)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews CROSS JOIN movies
            WHERE userId = $id AND movies.id = reviews.movieId";
        // command.CommandText = @"SELECT * FROM reviews WHERE userId = $id"; 
        command.Parameters.AddWithValue("$id" , user.id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
           Review review =  FillReviewWithMovies(reader);
            list.Add(review); 
        }
        reader.Close() ; 
        user.reviews = list;
        return user;
    }

    public Review FillReviewWithMovies(SqliteDataReader reader)
    {
        Movie movie = new Movie();
        movie.id = int.Parse(reader.GetString(6));
        movie.title = reader.GetString(7);
        movie.releaseDate = DateTime.Parse(reader.GetString(8));
        movie.genre = reader.GetString(9);
        // movie.starringJackieChan = bool.Parse(reader.GetString(9));
        Review review = ReadReview(reader);  
        review.movie = movie;
        return review;
    }

    public  Dictionary<double, int> GetReviewsForHistogram(User user)
    {
        Dictionary<double, int> reviewFrequency = new Dictionary<double, int>();
        List<Review> list = GetAllByAuthorId(user.id);
        for( int i = 0; i < list.Count; i++)
        {
            Review current = list[i];
            if (!reviewFrequency.TryAdd(current.value,1))
            {
                reviewFrequency[list[i].value] += 1;
            }
        }
        return reviewFrequency;
    }
}