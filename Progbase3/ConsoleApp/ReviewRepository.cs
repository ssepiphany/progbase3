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

    public int DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
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
            INSERT INTO reviews(value, movieId, userId)
            VALUES($value, $movieId, $userId) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$value" , review.value ) ; 
        command.Parameters.AddWithValue("$movieId", review.movieId); 
        command.Parameters.AddWithValue("$userId", review.userId) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE reviews SET value = $value, movieId = $movieId, userId = $userId  WHERE id = $id" ; 
        command.Parameters.AddWithValue("$value", review.value); 
        command.Parameters.AddWithValue("$id", review.id) ; 
        command.Parameters.AddWithValue("$movieId", review.movieId); 
        command.Parameters.AddWithValue("$userId", review.userId) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    private Review ReadReview(SqliteDataReader reader)
    {
        Review review =  new Review(); 
        review.id = int.Parse(reader.GetString(0)) ; 
        review.value = double.Parse(reader.GetString(1)); 
        review.movieId = int.Parse(reader.GetString(2)) ; 
        review.userId = int.Parse(reader.GetString(3)) ; 
        return review ;
    }

    public User GetReviewsForExport(User user)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE userId = $id"; 
        command.Parameters.AddWithValue("$id" , user.id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader);  
            list.Add(review); 
        }
        reader.Close() ; 
        user.reviews = list;
        return user;
    }

    public struct HistogramData
    {
        public Dictionary<double, int> reviewFrequency;
        public List<double> reviewValues;
    }

    public HistogramData GetReviewsForHistogram(User user)
    {
        HistogramData hd = new HistogramData();
        hd.reviewValues= new List<double>();
        hd.reviewFrequency = new Dictionary<double, int>();
        List<Review> list = GetAllByAuthorId(user.id);
        for( int i = 0; i < list.Count; i++)
        {
            Review current = list[i];
            if (hd.reviewFrequency.TryAdd(current.value,1))
            {
                hd.reviewValues.Add(current.value);
            }
            else
            {
                hd.reviewFrequency[list[i].value] += 1;
            }
        }
        return hd;
    }
}