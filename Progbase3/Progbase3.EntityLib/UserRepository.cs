using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;

public class UserRepository
{
    private SqliteConnection connection;

    public UserRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public User GetById(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM users WHERE id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if ( reader.Read())
        {
            User user = ReadUser(reader); 
            // user.reviews = reviewRepo.GetAllByAuthorId(user.id);
            reader.Close(); 
            return user;
        } 
        else
        {
            reader.Close(); 
            return null; 
        } 
    }

    public User GetByLogin(string login)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM users WHERE login = $login";
        command.Parameters.AddWithValue("$login" , login) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if (reader.Read())
        {
            User user = ReadUser(reader); 
            reader.Close(); 
            return user;
        } 
        else
        {
            reader.Close(); 
            return null; 
        } 
    }

    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (float)pageLength) ; 
    }


    public List<User> GetPage(int pageNum, int pageLength)
    {
        if(pageNum < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<User> page = new List<User>();
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"SELECT * FROM users LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            User user = ReadUser(reader) ; 
            page.Add(user); 
        }
        reader.Close() ; 
        return page ; 
    }


    public List<User> GetAll(ReviewRepository reviewRepo)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM users"; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<User> list = new List<User>() ; 
        while(reader.Read())
        {
            User user = ReadUser(reader) ;  
            user.reviews = reviewRepo.GetAllByAuthorId(user.id);
            list.Add(user); 
        }
        reader.Close() ; 
        return list ;  
    }

    public User GetByReviewId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM users CROSS JOIN reviews
            WHERE users.id = reviews.movieId AND reviews.id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if ( reader.Read())
        {
            User user = ReadUser(reader) ; 
            reader.Close() ; 
            return user ;
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
        command.CommandText = @"DELETE FROM users WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return (res == 1) ; 
    }

    public int DeleteByReviewId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ;  
        command.CommandText = @"DELETE FROM users CROSS JOIN reviews
            WHERE users.id = reviews.userId AND reviews.id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int Insert(User user)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO users(login , fullname , createdAt, password, moderator)
            VALUES( $login, $fullname, $createdAt, $password, $moderator); 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$login" , user.login) ; 
        command.Parameters.AddWithValue("$fullname" , user.fullname) ; 
        command.Parameters.AddWithValue("$createdAt" , user.createdAt.ToString("o")) ; 
        command.Parameters.AddWithValue("$password" , user.password) ; 
        command.Parameters.AddWithValue("$moderator" , user.moderator.ToString()) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, User user)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE users SET login = $login , fullname = $fullname, password = $password, moderator = $moderator WHERE id = $id" ; 
        command.Parameters.AddWithValue("$login" , user.login); 
        command.Parameters.AddWithValue("$fullname" , user.fullname); 
        command.Parameters.AddWithValue("$id" , user.id); 
        command.Parameters.AddWithValue("$password" , user.password); 
        command.Parameters.AddWithValue("$moderator" , user.moderator.ToString()) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    private User ReadUser(SqliteDataReader reader)
    {
        User user =  new User() ; 
        user.id = int.Parse(reader.GetString(0)); 
        user.login = reader.GetString(1);
        user.fullname = reader.GetString(2);  
        user.createdAt = DateTime.Parse(reader.GetString(3));  
        user.password = reader.GetString(4);
        user.moderator = bool.Parse(reader.GetString(5));
        return user ;
    }

    public long GetCount()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM users"; 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public HashSet<string> GetAllLogins()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT login FROM users;"; 
        SqliteDataReader reader = command.ExecuteReader() ;
        HashSet<string> logins = new HashSet<string>(); 
        while(reader.Read())
        {
            logins.Add(reader.GetString(0));
        } 
        reader.Close() ; 
        return logins ;
    }
}