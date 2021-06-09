using System;
using System.Collections.Generic;

public class User
{
    public int id;
    public string login;
    public string fullname;
    public string password;
    public DateTime createdAt;
    public List<Review> reviews;
    public bool moderator;

    public override string ToString()
    {
        return $"[{this.id}] {this.fullname}: ({this.login})";
    }

}