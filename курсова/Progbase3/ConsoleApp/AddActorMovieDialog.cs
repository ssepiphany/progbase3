using Terminal.Gui;

public class AddActorMovieDialog : CreateMovieDialog
{
    public int selectedItem;
    public AddActorMovieDialog()
    {
        this.dialogTitle = "Add movie";
        this.Title = this.dialogTitle;

        NStack.ustring[] options2 = new NStack.ustring[]{ "create movie", "choose movie"};
        RadioGroup choiceGroup = new RadioGroup
        {
            X = 2, Y = 20, RadioLabels = options2, 
        };
        choiceGroup.SelectedItemChanged += OnSelectedItemChanged;

        this.Add(choiceGroup);
    }

    public string GetMovieTitle()
    {
        return this.movieTitleInput.Text.ToString();
    }

    protected void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        if(this.selectedItem == 0)
        {
            this.Add(genreGroup);
            this.Add(genreLbl);
            this.Add(releaseDateLbl);
            this.Add(dateInput);
        }
        if(this.selectedItem == 1)
        {
            this.Remove(genreGroup);
            this.Remove(genreLbl);
            this.Remove(releaseDateLbl);
            this.Remove(dateInput);
        }
    }

    protected override bool ValidateInput()
    {
        if(this.movieTitleInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        Movie chosenMovie = movieRepo.GetByTitle(movieTitleInput.Text.ToString());
        if(this.selectedItem == 1)
        {
            if(chosenMovie == null)
            {
                this.Title = MessageBox.ErrorQuery("Error", $"Movie \"{this.movieTitleInput .Text}\" does not exist", "OK").ToString();
                return false;
            }
        }
        if (this.selectedItem == 0)
        {
            if(chosenMovie != null)
            {
                this.Title = MessageBox.ErrorQuery("Error", $"Movie with title \"{this.movieTitleInput .Text}\"\r\nalready exists", "OK").ToString();
                return false;
            }
        }
        return true;
    }
}