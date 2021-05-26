using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("reviews")]
public class ReviewRoot
{
    [XmlElement("review")]
    public List<Review> reviews;
    [XmlAttribute()]
    public int userId; 

    public ReviewRoot()
    {
        this.reviews = new List<Review>();
    }
}