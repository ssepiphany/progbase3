using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;

public class ReviewRepository
{
    private SqliteConnection connection;

    public ReviewRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public Review GetById(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader(); 
        if ( reader.Read())
        {
            Review review = ReadReview(reader); 
            reader.Close(); 
            return review;
        } 
        else
        {
            reader.Close(); 
            return null; 
        } 
    }

    // public List<Review> GetPage(int pageNum, int pageLength)
    // {
    //     if(pageNum < 1)
    //     {
    //         throw new ArgumentOutOfRangeException();
    //     }
    //     int offset = pageLength * (pageNum - 1);
    //     List<Review> page = new List<Review>();
    //     SqliteCommand command = connection.CreateCommand() ;
    //     command.CommandText = @"SELECT * FROM reviews CROSS JOIN movies 
    //         WHERE movies.id = reviews.movieId LIMIT $pageLength OFFSET $offset" ; 
    //     command.Parameters.AddWithValue("$pageLength" , pageLength) ;
    //     command.Parameters.AddWithValue("$offset" , offset) ;
    //     SqliteDataReader reader = command.ExecuteReader() ; 
    //     while(reader.Read())
    //     {
    //         Review review = FillReviewWithMovies(reader); 
    //         page.Add(review); 
    //     }
    //     reader.Close() ; 
    //     return page ; 
    // }

    public List<Review> GetAuthorPage(int pageNum, int pageLength, int id)
    {
        if(pageNum < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Review> page = new List<Review>();
        SqliteCommand command = connection.CreateCommand() ;
        command.CommandText = @"SELECT * FROM reviews CROSS JOIN movies 
            WHERE movies.id = reviews.movieId AND reviews.userId = $id LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        command.Parameters.AddWithValue("$id" , id) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Review review = FillReviewWithMovies(reader); 
            review.outputFormat = 1;
            page.Add(review); 
        }
        reader.Close() ; 
        return page ; 
    }

    public long GetCountForMovie(int movieId)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM reviews WHERE movieId = $movieId"; 
        command.Parameters.AddWithValue("$movieId" , movieId); 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public List<Review> GetPageForMovie(int pageNum, int pageLength, int id)
    {
        if(pageNum < 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        int offset = pageLength * (pageNum - 1);
        List<Review> page = new List<Review>();
        SqliteCommand command = connection.CreateCommand() ;
        command.CommandText = @"SELECT * FROM reviews CROSS JOIN movies, users 
            WHERE movies.id = reviews.movieId AND movieId = $id AND users.id = reviews.userId LIMIT $pageLength OFFSET $offset" ; 
        command.Parameters.AddWithValue("$pageLength" , pageLength) ;
        command.Parameters.AddWithValue("$offset" , offset) ;
        command.Parameters.AddWithValue("$id" , id) ;
        SqliteDataReader reader = command.ExecuteReader() ; 
        while(reader.Read())
        {
            Review review = FillReviewWithMovies(reader); 
            review.author = ReadAuthor(reader);
            review.outputFormat = 2;
            page.Add(review); 
        }
        reader.Close() ; 
        return page ; 
    }

    private User ReadAuthor(SqliteDataReader reader)
    {
        User user = new User();
        user.id = int.Parse(reader.GetString(10));
        user.login = reader.GetString(11);
        user.fullname = reader.GetString(12);
        user.createdAt = DateTime.Parse(reader.GetString(13));
        user.password = reader.GetString(14);
        user.moderator = bool.Parse(reader.GetString(15));
        return user;
    }

    // public int GetTotalPages(int pageLength)
    // {
    //     return (int)Math.Ceiling(this.GetCount() / (float)pageLength) ; 
    // }

    public int GetTotalPagesForAuthor(int pageLength, int id)
    {
        return (int)Math.Ceiling(this.GetCountForAuthor(id) / (float)pageLength) ; 
    }

    public int GetTotalPagesForMovie(int pageLength, int movieId)
    {
        return (int)Math.Ceiling(this.GetCountForMovie(movieId) / (float)pageLength) ; 
    }

    public long GetCountForAuthor(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM reviews WHERE userId = $userId"; 
        command.Parameters.AddWithValue("$userId" , id); 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public List<Review> GetAll()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews"; 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader) ;  
            list.Add(review); 
        }
        reader.Close() ; 
        return list ;  
    }

    public List<Review> GetAllByAuthorId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE userId = $id"; 
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader);  
            list.Add(review); 
        }
        reader.Close() ; 
        return list ;  
    }

    public List<Review> GetAllByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews WHERE movieId = $id"; 
        command.Parameters.AddWithValue("$id" , id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
            Review review = ReadReview(reader);  
            list.Add(review); 
        }
        reader.Close() ; 
        return list ;  
    }

    public bool DeleteById(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE id = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1 ; 
    }

    public bool DeleteAllByAuthorId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE userId = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1; 
    }

    public bool DeleteAllByMovieId(int id)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"DELETE FROM reviews WHERE movieId = $id" ; 
        command.Parameters.AddWithValue("$id" , id) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1; 
    }


    public int Insert(Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText =
        @"
            INSERT INTO reviews(value, movieId, userId, createdAt, imported)
            VALUES($value, $movieId, $userId, $createdAt, $imported); 
            SELECT last_insert_rowid() ; 
        ";
        command.Parameters.AddWithValue("$value" , review.value ) ; 
        command.Parameters.AddWithValue("$movieId", review.movieId); 
        command.Parameters.AddWithValue("$userId", review.userId) ; 
        command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o")) ;
        command.Parameters.AddWithValue("$imported", review.imported.ToString()) ; 
        long newId = (long)command.ExecuteScalar() ;
        return (int)newId ; 
    }

    public bool Update(int id, Review review)
    {
        SqliteCommand command = connection.CreateCommand() ; 
        command.CommandText = @"UPDATE reviews SET value = $value, movieId = $movieId, 
            userId = $userId, createdAt = $createdAt, imported = $imported WHERE id = $id" ; 
        command.Parameters.AddWithValue("$value", review.value); 
        command.Parameters.AddWithValue("$id", review.id) ; 
        command.Parameters.AddWithValue("$movieId", review.movieId); 
        command.Parameters.AddWithValue("$userId", review.userId) ; 
        command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o")) ; 
        command.Parameters.AddWithValue("$imported", review.imported.ToString()) ; 
        int res = command.ExecuteNonQuery() ; 
        return res == 1;
    }

    private Review ReadReview(SqliteDataReader reader)
    {
        Review review =  new Review(); 
        review.id = int.Parse(reader.GetString(0)) ; 
        review.value = int.Parse(reader.GetString(1)); 
        review.movieId = int.Parse(reader.GetString(2)) ; 
        review.userId = int.Parse(reader.GetString(3)) ;
        review.createdAt = DateTime.Parse(reader.GetString(4)); 
        review.imported = bool.Parse(reader.GetString(5));
        return review ;
    }

    public long GetCount()
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT COUNT(*) FROM reviews"; 
        long count = (long)command.ExecuteScalar();
        return count;
    }

    public User GetReviewsForExport(User user)
    {
        SqliteCommand command = connection.CreateCommand(); 
        command.CommandText = @"SELECT * FROM reviews CROSS JOIN movies
            WHERE userId = $id AND movies.id = reviews.movieId";
        // command.CommandText = @"SELECT * FROM reviews WHERE userId = $id"; 
        command.Parameters.AddWithValue("$id" , user.id); 
        SqliteDataReader reader = command.ExecuteReader() ; 
        List<Review> list = new List<Review>() ; 
        while(reader.Read())
        {
           Review review =  FillReviewWithMovies(reader);
            list.Add(review); 
        }
        reader.Close() ; 
        user.reviews = list;
        return user;
    }

    public Review FillReviewWithMovies(SqliteDataReader reader)
    {
        Movie movie = new Movie();
        movie.id = int.Parse(reader.GetString(6));
        movie.title = reader.GetString(7);
        movie.releaseDate = DateTime.Parse(reader.GetString(8));
        movie.genre = reader.GetString(9);
        // movie.starringJackieChan = bool.Parse(reader.GetString(9));
        Review review = ReadReview(reader);  
        review.movie = movie;
        return review;
    }

    public  Dictionary<int, int> GetReviewsForHistogram(User user)
    {
        Dictionary<int, int> reviewFrequency = new Dictionary<int, int>();
        List<Review> list = GetAllByAuthorId(user.id);
        for( int i = 0; i < list.Count; i++)
        {
            Review current = list[i];
            if (!reviewFrequency.TryAdd(current.value,1))
            {
                reviewFrequency[current.value] += 1;
            }
        }
        return reviewFrequency;
    }

    public double GetUserAverageScore(User user)
    {
        List<Review> reviews = this.GetAllByAuthorId(user.id);
        int sum = 0; 
        for (int i = 0; i < reviews.Count; i++)
        {
            sum += reviews[i].value;
        }
        double res = Math.Round((double) sum / reviews.Count, 1);
        return res;
    }

    public Review GetUserHighestScoreReview(List<Review> reviews)
    {
        Sort(reviews, 0, reviews.Count - 1);
        return reviews[reviews.Count - 1];
    }

    public Review GetUserLowestScoreReview(List<Review> reviews)
    {
        Sort(reviews, 0, reviews.Count - 1);
        return reviews[0];
    }

    private void Sort(List<Review> reviews , int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(reviews, low, high);
            Sort(reviews, low, partitionIndex - 1);
            Sort(reviews, partitionIndex + 1, high);
        }
    }

    private int Partition(List<Review> reviews, int low, int high)
    {
        Review pivot = reviews[high];
        int lowIndex = (low - 1);
        for (int j = low; j < high; j++)
        {
            if (reviews[j].value <= pivot.value)
            {
                lowIndex++;

                Review temp = reviews[lowIndex];
                reviews[lowIndex] = reviews[j];
                reviews[j] = temp;
            }
        }

        Review temp1 = reviews[lowIndex + 1];
        reviews[lowIndex + 1] = reviews[high];
        reviews[high] = temp1;
        return lowIndex + 1;
    }
}