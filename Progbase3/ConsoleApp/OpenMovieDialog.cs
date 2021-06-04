using Terminal.Gui;

public class OpenMovieDialog : Dialog
{
    public bool deleted;
    public bool updated;
    private Movie movie;
    private TextField idInput;
    private TextField movieTitleInput;
    private TextField genreGroup;
    private TextField dateInput;
    private User currentUser;
    private Button editBtn;
    private Button deleteBtn;
    private ActorRepository actorRepo;
    private ReviewRepository reviewRepo;
    private MovieRepository movieRepo;
    private UserRepository userRepo;
    public OpenMovieDialog()
    {
        this.Title = "Open movie";

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogSubmit;

        this.AddButton(backBtn);

        int rightColumn = 20;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(idLbl, idInput);

        Label movieTitleLbl = new Label(2, 4, "Title:");
        movieTitleInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(movieTitleLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(movieTitleLbl, movieTitleInput);

        Label genreLbl = new Label(2, 6, "Genre:");
        genreGroup = new TextField()
        {
            X = rightColumn, Y = Pos.Top(genreLbl), Width = 40, ReadOnly = true, 
        };

        this.Add(genreLbl, genreGroup);

        Label releaseDateLbl = new Label(2, 8, "Release date:");
        dateInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(releaseDateLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(releaseDateLbl, dateInput);

        editBtn = new Button(2, 22, "Edit");
        editBtn.Clicked += OnMovieEdit;
        this.Add(editBtn);

        deleteBtn = new Button("Delete")
        {
            X = Pos.Right(editBtn) + 2, Y = Pos.Top(editBtn), 
        };
        deleteBtn.Clicked += OnMovieDelete;
        this.Add(deleteBtn);

        Button castBtn = new Button()
        {
            X = 2, Y = Pos.Percent(55), Text = "View cast", Width = 13, 
        };
        castBtn.Clicked += OnViewCast;
        this.Add(castBtn);

        Button reviewsBtn = new Button()
        {
            X = 2, Y = Pos.Percent(65), Text = "View reviews", Width = 16, 
        };
        reviewsBtn.Clicked += OnViewReviews;
        this.Add(reviewsBtn);

    }

    private void OnMovieDelete()
    {
        int index = MessageBox.Query("Delete movie", "Are you sure", "No", "Yes");
        if(index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }

    public void SetRepositories(ActorRepository actorRepo, ReviewRepository reviewRepo, UserRepository userRepo, MovieRepository movieRepo)
    {
        this.actorRepo = actorRepo;
        this.reviewRepo = reviewRepo;
        this.userRepo = userRepo;
        this.movieRepo = movieRepo;
    }

    private void OnViewCast()
    {
        OpenCastWindow win = new OpenCastWindow();
        win.SetRepositories(actorRepo, movieRepo);
        win.SetMovie(this.movie);
        win.SetCurrentUser(this.currentUser);
        Application.Run(win);
    }

    private void OnViewReviews()
    {
        OpenMovieReviewsWindow win = new OpenMovieReviewsWindow();
        win.SetRepositories(movieRepo, userRepo, reviewRepo);
        win.SetMovie(this.movie);
        win.SetCurrentUser(this.currentUser);
        Application.Run(win);
    }

    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
        this.editBtn.Visible = this.currentUser.moderator;
        this.deleteBtn.Visible = this.currentUser.moderator;
    }

    private void OnMovieEdit()
    {
        EditMovieDialog dialog = new EditMovieDialog();
        dialog.SetRepository(movieRepo);
        dialog.SetMovie(this.movie);
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Movie updatedMovie = dialog.GetMovie();
            this.updated = true;
            updatedMovie.id = movie.id;
            this.SetMovie(updatedMovie);
        }
    }

    public Movie GetMovie()
    {
        return this.movie;
    }

    public void SetMovie(Movie movie)
    {
        this.movie = movie;
        this.idInput.Text = movie.id.ToString();
        this.movieTitleInput.Text = movie.title;
        this.genreGroup.Text = movie.genre.ToString();
        this.dateInput.Text = movie.releaseDate.ToString("F");
    }

    private void OnOpenDialogSubmit()
    {
        Application.RequestStop();
    }
}