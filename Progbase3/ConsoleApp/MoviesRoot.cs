using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("movies")]
public class MovieRoot
{
    [XmlElement("movie")]
    public List<Movie> movies;

    public MovieRoot()
    {
        this.movies = new List<Movie>();
    }
}