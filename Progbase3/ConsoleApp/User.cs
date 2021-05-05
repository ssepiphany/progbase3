using System;
using System.Collections.Generic;

public class User
{
    public int id;
    public string login;
    public string fullname;
    public DateTime createdAt;
    public List<Review> reviews;
}