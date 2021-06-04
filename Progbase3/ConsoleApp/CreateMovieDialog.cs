using System;
using Terminal.Gui;

public class CreateMovieDialog : Dialog
{
    public bool canceled; 
    protected string dialogTitle;
    protected TextField movieTitleInput;
    protected RadioGroup genreGroup;
    protected NStack.ustring[] options;
    protected Label releaseDateLbl;

    protected DateField dateInput;
    protected Label genreLbl;
    protected MovieRepository movieRepo;
    public CreateMovieDialog()
    {
        this.dialogTitle = "Create movie";
        this.Title = this.dialogTitle;
        Button okBtn = new Button("OK");
        okBtn.Clicked += OnCreateDialogSubmit;

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCreateDialogCanceled;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        int rightColumn = 20;

        Label movieTitleLbl = new Label(2, 4, "Title:");
        movieTitleInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(movieTitleLbl), Width = 40
        };
        this.Add(movieTitleLbl, movieTitleInput);

        genreLbl = new Label(2, 6, "Genre:");
        options = new NStack.ustring[]{"comedy", "action", "drama", "horror", "fantasy", "other"};
        genreGroup = new RadioGroup(rightColumn, 6, options);

        this.Add(genreLbl, genreGroup);

        releaseDateLbl = new Label(2, 9 + options.Length, "Release date:");
        dateInput = new DateField()
        {
           X = rightColumn, Y = Pos.Top(releaseDateLbl), IsShortFormat = false, Width = 40,
        };
        this.Add(releaseDateLbl, dateInput);

    }

    public void SetRepository(MovieRepository movieRepo)
    {
        this.movieRepo = movieRepo;
    }

    public Movie GetMovie()
    {
        Movie movie = new Movie();
        movie.title = this.movieTitleInput.Text.ToString();
        movie.genre = options[genreGroup.SelectedItem].ToString();
        movie.releaseDate = this.dateInput.Date;
        return movie;
    }

    protected void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }

    protected void OnCreateDialogSubmit()
    {
        if(!ValidateInput()) 
        {
            this.Title = this.dialogTitle; 
            return;
        }
        this.canceled = false;
        Application.RequestStop();
    }

    protected virtual bool ValidateInput()
    {
        if(this.movieTitleInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        if(movieRepo.GetByTitle(this.movieTitleInput.Text.ToString()) != null)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Movie with title \"{this.movieTitleInput.Text}\"\r\nalready exists", "OK").ToString();
            return false;
        }
        return true;
    }
}