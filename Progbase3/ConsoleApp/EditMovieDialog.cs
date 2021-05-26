using Terminal.Gui;

public class EditMovieDialog : CreateMovieDialog
{
    
    private TextField idInput;
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
        this.idInput.Text = movie.id.ToString();
        this.movieTitleInput.Text = movie.title;
        this.genreGroup.SelectedItem = GetIndexOfSelectedItem(movie.genre);
        this.dateInput.Date = movie.releaseDate;
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