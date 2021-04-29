using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

public class MovieRepository
{
    private SqliteConnection connection;

    public MovieRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public Movie GetById(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM movies WHERE id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if ( reader.Read())
        {
            Movie movie = ReadMovie(reader) ; 
            reader.Close() ; 
            return movie ;
        } 
        else
        {
            reader.Close() ; 
            return null; 
        } 
    }

    public List<Movie> GetAll()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM movies"; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Movie> list = new List<Movie>() ; 
        while(reader.Read())
        {
            Movie movie = ReadMovie(reader) ;  
            list.Add(movie); 
        }
        reader.Close() ; 
        return list ;  
    }

    public int DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM movies WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int Insert(Movie movie)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO movies(title, releaseDate)
            VALUES( $title, $releaseDate) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$title" , movie.title ) ; 
        command.Parameters.AddWithValue("$releaseDate" , movie.releaseDate.ToString("o") ) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Movie movie)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE movies SET title = $title , releaseDate = $releaseDate WHERE id = $id" ; 
        command.Parameters.AddWithValue("$title", movie.title); 
        command.Parameters.AddWithValue("$releaseDate", movie.releaseDate) ; 
        command.Parameters.AddWithValue("$id", movie.id); 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    private Movie ReadMovie(SqliteDataReader reader)
    {
        Movie movie =  new Movie(); 
        movie.id = int.Parse(reader.GetString(0)) ; 
        movie.title = reader.GetString(1) ;
        movie.releaseDate = DateTime.Parse(reader.GetString(2)) ;  
        return movie ;
    }
}