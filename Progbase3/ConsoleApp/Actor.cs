using System.Collections.Generic;

public class Actor
{
    public int id;
    public string fullname;  
    public int age;
    public string gender;
    public List<Movie> movies;

    public override string ToString()
    {
        return $"[{this.id}] {this.fullname} ({this.age})";
    }
}