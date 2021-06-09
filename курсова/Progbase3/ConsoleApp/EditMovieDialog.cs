using Terminal.Gui;

public class EditMovieDialog : CreateMovieDialog
{
    
    private TextField idInput;
    protected Movie movie;
    public EditMovieDialog()
    {
        this.dialogTitle = "Edit movie";
        this.Title = this.dialogTitle;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = 20, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true
        };
        this.Add(idLbl, idInput);

        this.Add(dateInput);
    }

    public void SetMovie(Movie movie)
    {
        this.movie = movie; 
        this.idInput.Text = movie.id.ToString();
        this.movieTitleInput.Text = movie.title;
        this.genreGroup.SelectedItem = GetIndexOfSelectedItem(movie.genre);
        this.dateInput.Date = movie.releaseDate;
    }

    protected override bool ValidateInput()
    {
        if(this.movieTitleInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        if(movieRepo.GetByTitle(this.movieTitleInput.Text.ToString()) != null && this.movieTitleInput.Text.ToString() != this.movie.title)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Movie with title \"{this.movieTitleInput.Text}\"\r\nalready exists", "OK").ToString();
            return false;
        }
        return true;
    }

    private int GetIndexOfSelectedItem(string item)
    {
        for(int i = 0; i < options.Length; i++)
        {
            if(options[i] == item)
            {
                return i;
            }
        }
        return -1;
    }
}