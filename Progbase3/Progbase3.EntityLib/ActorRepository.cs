using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;

public class ActorRepository
{
    private SqliteConnection connection;

    public ActorRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public Actor GetById(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM actors WHERE id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if ( reader.Read())
        {
            Actor actor = ReadActor(reader) ; 
            reader.Close() ; 
            return actor ;
        } 
        else
        {
            reader.Close() ; 
            return null; 
        } 
    }

    public Actor GetByFullname(string fullname)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM actors WHERE fullname = $fullname";
        command.Parameters.AddWithValue("$fullname" , fullname) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if ( reader.Read())
        {
            Actor actor = ReadActor(reader) ; 
            reader.Close() ; 
            return actor ;
        } 
        else
        {
            reader.Close() ; 
            return null; 
        } 
    }

    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (float)pageLength) ; 
    }

    public int GetTotalPagesForMovie(int pageLength, int movieId)
    {
        return (int)Math.Ceiling(this.GetCountForMovie(movieId) / (float)pageLength) ;
    }

    public long GetCountForMovie(int movieId)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM movieActors WHERE movieId = $movieId"; 
        command.Parameters.AddWithValue("$movieId" , movieId) ;
        long count = (long)command.ExecuteScalar();
        return count;
    } 

    public List<Actor> GetPage(int pageNum, int pageLength)
    {
        if(pageNum < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Actor> page = new List<Actor>();
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"SELECT * FROM actors LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Actor actor = ReadActor(reader) ; 
            page.Add(actor); 
        }
        reader.Close() ; 
        return page ; 
    }

    public List<Actor> GetPageForMovie(int pageNum, int pageLength, int movieId)
    {
        if(pageNum < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Actor> page = new List<Actor>();
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"SELECT * FROM actors, movies, movieActors
            WHERE movieActors.movieId = movies.id
            AND movieActors.actorId = actors.Id
            AND movies.id = $id LIMIT $pageLength OFFSET $offset"; 
        command.Parameters.AddWithValue("$id" , movieId); 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Actor actor = ReadActor(reader) ; 
            page.Add(actor); 
        }
        reader.Close() ; 
        return page ; 
    }

    public List<Actor> GetAll()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM actors"; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Actor> list = new List<Actor>() ; 
        while(reader.Read())
        {
            Actor actor = ReadActor(reader) ;  
            list.Add(actor); 
        }
        reader.Close() ; 
        return list ;  
    }

    public List<Actor> GetByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM actors, movies, movieActors
            WHERE movieActors.movieId = movies.id
            AND movieActors.actorId = actors.Id
            AND movies.id = $id"; 
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Actor> list = new List<Actor>() ; 
        while(reader.Read())
        {
            Actor actor = ReadActor(reader) ;  
            list.Add(actor); 
        }
        reader.Close() ; 
        return list ;  
    }

    public bool DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM actors WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    public int DeleteByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM actors, movies, movieActors
            WHERE movieActors.movieId = movies.id 
            AND movieActors.actorId = actors.Id
            AND movies.id = $id";  
        command.Parameters.AddWithValue("$id" , id ); 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int Insert(Actor actor)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO actors(fullname, age, gender)
            VALUES( $fullname, $age, $gender) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$fullname" , actor.fullname); 
        command.Parameters.AddWithValue("$age" , actor.age); 
        command.Parameters.AddWithValue("$gender" , actor.gender); 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Actor actor)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE actors SET fullname = $fullname, age = $age, gender = $gender WHERE id = $id" ; 
        command.Parameters.AddWithValue("$fullname", actor.fullname); 
        command.Parameters.AddWithValue("$age" , actor.age); 
        command.Parameters.AddWithValue("$gender" , actor.gender); 
        command.Parameters.AddWithValue("$id", id); 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    public long GetCount()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM actors"; 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    private Actor ReadActor(SqliteDataReader reader)
    {
        Actor actor =  new Actor() ; 
        actor.id = int.Parse(reader.GetString(0)) ; 
        actor.fullname = reader.GetString(1) ; 
        actor.age = int.Parse(reader.GetString(2));
        actor.gender = reader.GetString(3) ; 
        return actor ;
    }

    public bool RemoveConnectionsWithActor(int actorId)
    {
        SqliteCommand command = connection.CreateCommand() ;  
        command.CommandText = @"DELETE FROM movieActors
            WHERE actorId = $actorId";
        command.Parameters.AddWithValue("$actorId" , actorId) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1; 
    }
}