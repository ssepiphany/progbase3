using System.Xml.Serialization;
using System;

public class Review
{
    public int id;
    public int value;
    [XmlElement("movie_id")]
    public int movieId;
    public string comment;
    [XmlElement("created_at")]
    public DateTime createdAt;
    [XmlIgnore]
    public int userId;
    [XmlIgnore]
    public User author;
    [XmlIgnore]
    public Movie movie;
    [XmlIgnore]
    public bool imported;

    public override string ToString()
    {
        return $"[{this.id}] \"{movie.title}\": {this.value} points";
    }
}