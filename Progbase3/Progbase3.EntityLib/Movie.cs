using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Movie
{
    public int id;
    public string title;
    [XmlElement("release_date")]
    public DateTime releaseDate;
    public string genre;
    [XmlIgnore]
    public List<Review> reviews;
    [XmlIgnore]
    public List<Actor> actors;
    public override string ToString()
    {
        return $"[{this.id}] \"{this.title}\" ({this.genre})";
    }
}
