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

    public Review GetById(int id )
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

    public int DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int Insert(Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO reviews(value)
            VALUES($value) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$value" , review.value ) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE reviews SET value = $value WHERE id = $id" ; 
        command.Parameters.AddWithValue("$value", review.value); 
        command.Parameters.AddWithValue("$id", review.id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    private Review ReadReview(SqliteDataReader reader)
    {
        Review review =  new Review(); 
        review.id = int.Parse(reader.GetString(0)) ; 
        review.value = double.Parse(reader.GetString(1)); 
        return review ;
    }


}