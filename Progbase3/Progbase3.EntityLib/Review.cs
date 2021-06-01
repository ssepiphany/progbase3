using System.Xml.Serialization;
using System;

public class Review
{
    public int id;
    public int value;
    [XmlElement("movie_id")]
    public int movieId;
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
    [XmlIgnore]
    public int outputFormat;

    public override string ToString()
    {
        if(this.outputFormat == 1)
        {
            return $"[{this.id}] \"{this.movie.title}\": {this.value}/10";
        }
        return $"[{this.id}] {this.author.login}: {this.value}/10";
    }
}