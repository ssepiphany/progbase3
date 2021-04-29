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

    public User GetById(int id )
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM users WHERE id = $id";
        command.Parameters.AddWithValue("$id" , id) ; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        if ( reader.Read())
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

    public List<User> GetAll()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM users"; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<User> list = new List<User>() ; 
        while(reader.Read())
        {
            User user = ReadUser(reader) ;  
            list.Add(user); 
        }
        reader.Close() ; 
        return list ;  
    }

    public int DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM users WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res ; 
    }

    public int Insert(User user)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO users(login , fullname , createdAt )
            VALUES( $login, $fullname, $createdAt) ; 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$login" , user.login) ; 
        command.Parameters.AddWithValue("$fullname" , user.fullname) ; 
        command.Parameters.AddWithValue("$createdAt" , user.createdAt.ToString("o")) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, User user)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE users SET login = $login , fullname = $fullname ,  WHERE id = $id" ; 
        command.Parameters.AddWithValue("$login" , user.login) ; 
        command.Parameters.AddWithValue("$fullname" , user.fullname) ; 
        command.Parameters.AddWithValue("$id" , user.id) ; 
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
        return user ;
    }
}