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

    public Movie GetByTitle(string title)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM movies WHERE title = $title";
        command.Parameters.AddWithValue("$title" , title) ; 
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

    public Movie GetByReviewId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM movies CROSS JOIN reviews
            WHERE movies.id = reviews.movieId AND reviews.id = $id";
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

    public bool DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM movies WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1 ; 
    }

    public int DeleteByActorId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM actors, movies, movieActors
            WHERE movieActors.movieId = movies.id
            AND movieActors.actorId = actors.Id
            AND actor.id = $id"; 
        command.Parameters.AddWithValue("$id" , id); 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int DeleteByReviewId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ;  
        command.CommandText = @"DELETE FROM movies CROSS JOIN reviews
            WHERE movies.id = reviews.movieId AND reviews.id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int Insert(Movie movie)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO movies(title, releaseDate, genre)
            VALUES( $title, $releaseDate, $genre) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$title" , movie.title ); 
        command.Parameters.AddWithValue("$releaseDate" , movie.releaseDate.ToString("o")); 
        command.Parameters.AddWithValue("$genre" , movie.genre ); 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public int GetTotalPages(int pageLength)
    {
        if(pageLength < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        return (int)Math.Ceiling(this.GetCount() / (float)pageLength) ; 
    }

    public int GetTotalPagesForActor(int pageLength, int actorId)
    {
        if(pageLength < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        return (int)Math.Ceiling(this.GetCountForActor(actorId) / (float)pageLength) ; 
    }

    public List<Movie> GetPage(int pageNum, int pageLength)
    {
        if(pageNum < 1 || pageLength < 1)
        {
            throw new ArgumentOutOfRangeException();
        }

        int offset = pageLength * (pageNum - 1);
        List<Movie> page = new List<Movie>();
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"SELECT * FROM movies LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Movie movie = ReadMovie(reader) ; 
            page.Add(movie); 
        }
        reader.Close() ; 
        return page ; 
    }

    public List<Movie> GetPageForActor(int pageNum, int pageLength, int actorId)
    {
        if(pageNum < 1 || pageLength < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Movie> page = new List<Movie>();
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"SELECT * FROM movies CROSS JOIN movieActors 
            WHERE movies.id = movieActors.movieId AND actorId = $actorId
            LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$actorId" , actorId); 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Movie movie = ReadMovie(reader) ; 
            page.Add(movie); 
        }
        reader.Close() ; 
        return page ; 
    }

    public bool Update(int id, Movie movie)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE movies SET title = $title , releaseDate = $releaseDate, genre = $genre WHERE id = $id" ; 
        command.Parameters.AddWithValue("$title", movie.title); 
        command.Parameters.AddWithValue("$releaseDate", movie.releaseDate) ; 
        command.Parameters.AddWithValue("$genre", movie.genre) ; 
        // command.Parameters.AddWithValue("$starringJackieChan", movie.starringJackieChan) ; 
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
        movie.genre = reader.GetString(3);
        return movie ;
    }

    public long GetCount()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM movies"; 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public long GetCountForActor(int actorId)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM movies CROSS JOIN movieActors
            WHERE movies.id = movieActors.movieId AND actorId = $actorId"; 
        command.Parameters.AddWithValue("$actorId" , actorId); 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public int GetSearchPagesCount(string searchValue, int pageLength)
    {
        if (string.IsNullOrEmpty(searchValue))
        {
            return this.GetTotalPages(pageLength);
        }
        if(pageLength < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM movies WHERE title LIKE '%' || $searchValue || '%' 
            OR genre LIKE '%' || $searchValue || '%'"; 
        command.Parameters.AddWithValue("$searchValue" , searchValue); 
        long count = (long)command.ExecuteScalar();
        return (int)Math.Ceiling(count / (float)pageLength) ; 
    }

    public List<Movie> GetSearchPage(string searchValue, int pageNum, int pageLength)
    {
        if (string.IsNullOrEmpty(searchValue))
        {
            return this.GetPage(pageNum, pageLength);
        }
        if(pageNum < 1 || pageLength < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Movie> page = new List<Movie>();
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM movies WHERE title LIKE '%' || $searchValue || '%' 
             OR genre LIKE '%' || $searchValue || '%' LIMIT $pageLength OFFSET $offset"; 
        command.Parameters.AddWithValue("$searchValue" , searchValue); 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Movie movie = ReadMovie(reader) ; 
            page.Add(movie); 
        }
        reader.Close() ; 
        return page ; 
    }

    public int ConnectMovieActor(int movieId, int actorId)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO movieActors(movieId, actorId)
            VALUES( $movieId, $actorId) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$movieId" , movieId); 
        command.Parameters.AddWithValue("$actorId" , actorId); 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool RemoveConnectionsWithMovie(int movieId)
    {
        SqliteCommand command = connection.CreateCommand() ;  
        command.CommandText = @"DELETE FROM movieActors
            WHERE movieId = $movieId";
        command.Parameters.AddWithValue("$movieId" , movieId) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1; 
    }
}