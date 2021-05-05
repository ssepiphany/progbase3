using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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

    public int DeleteByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE * FROM actors, movies, movieActors
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
            INSERT INTO actors(fullname)
            VALUES( $fullname) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$fullname" , actor.fullname); 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Actor actor)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE actors SET fullname = $fullname WHERE id = $id" ; 
        command.Parameters.AddWithValue("$fullname", actor.fullname); 
        command.Parameters.AddWithValue("$id", actor.id); 
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
        return actor ;
    }
}