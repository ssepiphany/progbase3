using System.Collections.Generic;
using System.IO.Compression;
using System.Xml.Serialization;
using System.IO;

public static class ExportImport
{
    public static void Export(User user, string dirPath)
    {
        string startPath = @"./xmlFiles";
        List<Review> reviews = user.reviews;
        ReviewRoot rRoot = new ReviewRoot();
        rRoot.userId = reviews[0].userId;
        rRoot.reviews = reviews;
        SerializeData<ReviewRoot>(rRoot, startPath + "/reviews.xml");
        MovieRoot mRoot = new MovieRoot();
        mRoot.movies = GetMovieList(ref reviews);;
        SerializeData<MovieRoot>(mRoot, startPath + "/movies.xml");

       string zipPath = dirPath + @"/result.zip";
       if(!Directory.Exists(dirPath))
       {
           Directory.CreateDirectory(dirPath);
       }
       bool exported = false;
       int c = 2;

       while(!exported)
       {
           try
            {
                ZipFile.CreateFromDirectory(startPath, zipPath);
                exported = true;
            }
            catch
            {
                zipPath = dirPath + $"/result({c}).zip";
                c++;
            }
       }

    }

    public static ReviewRoot Import(string zipPath)
    {
        try
        {
            string extractPath = Path.GetDirectoryName(zipPath) + "/import";
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            ReviewRoot root = new ReviewRoot();
            XmlSerializer ser = new XmlSerializer(typeof(ReviewRoot));
            System.IO.StreamReader reader = new System.IO.StreamReader(extractPath + "/reviews.xml");
            root = (ReviewRoot)ser.Deserialize(reader);
            reader.Close();
            Directory.Delete(extractPath, true);
            return root;
        }
        catch
        {
            return null; 
        }
    }

    private static void SerializeData<T>(T root, string filePath)
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));
        System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
        ser.Serialize(writer, root);
        writer.Close();
    }

    private static List<Movie> GetMovieList(ref List<Review> reviews)
    {
        List<Movie> movies = new List<Movie>();
        for(int i = 0; i < reviews.Count; i++)
        {
            movies.Add(reviews[i].movie);
        }
        return movies;
    }
}