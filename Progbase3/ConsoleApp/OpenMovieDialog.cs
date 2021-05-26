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

        Button editBtn = new Button(2, 22, "Edit");
        editBtn.Clicked += OnMovieEdit;
        this.Add(editBtn);

        Button deleteBtn = new Button("Delete")
        {
            X = Pos.Right(editBtn) + 2, Y = Pos.Top(editBtn), 
        };
        deleteBtn.Clicked += OnMovieDelete;
        this.Add(deleteBtn);
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

    private void OnMovieEdit()
    {
        EditMovieDialog dialog = new EditMovieDialog();
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