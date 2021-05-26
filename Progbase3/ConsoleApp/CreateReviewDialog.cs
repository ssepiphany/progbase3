using System;
using Terminal.Gui;


public class CreateReviewDialog : Dialog
{
    public bool canceled; 
    protected string dialogTitle;
    protected TextField scoreInput;
    protected DateField dateInput;
    protected TimeField timeInput;
    protected TextField movieIdInput;
    public MovieRepository movieRepo;
    public UserRepository userRepo;
    protected TextField userIdInput;
    protected Label createdAtLbl;
    public CreateReviewDialog()
    {
        this.dialogTitle = "Create review";
        this.Title = this.dialogTitle;
        Button okBtn = new Button("OK");
        okBtn.Clicked += OnCreateDialogSubmit;

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCreateDialogCanceled;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        int rightColumn = 20;

        Label scoreLbl = new Label(2, 4, "Score:");
        scoreInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(scoreLbl), Width = 40
        };
        this.Add(scoreLbl, scoreInput);

        Label movieIdLbl = new Label(2, 6, "Movie id:");
        movieIdInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(movieIdLbl), Width = 40
        };
        this.Add(movieIdLbl, movieIdInput); 

        Label userIdLbl = new Label(2, 8, "User id:");
        userIdInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(userIdLbl), Width = 40
        };
        this.Add(userIdLbl, userIdInput);        

        createdAtLbl = new Label(2, 10, "Created at:");
        dateInput = new DateField()
        {
            X = rightColumn, Y = Pos.Top(createdAtLbl), Date = DateTime.Now, Width = 12, ReadOnly = true,
        };

        timeInput = new TimeField()
        {
            X = rightColumn + 12, Y = Pos.Top(createdAtLbl), Time = DateTime.Now.TimeOfDay, Width = 28, ReadOnly = true,
        };

        this.Add(createdAtLbl, dateInput, timeInput);

    }

    public Review GetReview()
    {
        Review review = new Review();
        review.value = int.Parse(scoreInput.Text.ToString());
        review.movieId = int.Parse(movieIdInput.Text.ToString());
        review.createdAt = dateInput.Date + timeInput.Time;
        review.userId = int.Parse(userIdInput.Text.ToString());
        review.imported = false;
        return review;
    }

    public void SetRepositories(ref MovieRepository movieRepo, ref UserRepository userRepo)
    {
        this.movieRepo = movieRepo;
        this.userRepo = userRepo;
    }

    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }

    private void OnCreateDialogSubmit()
    {
        if(!ValidateInput()) 
        {
            this.Title = this.dialogTitle; 
            return;
        }
        this.canceled = false;
        Application.RequestStop();
    }

    public bool ValidateInput()
    {
        if(this.scoreInput.Text.IsEmpty || this.movieIdInput.Text.IsEmpty || this.userIdInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to input all fields", "OK").ToString();
            return false;
        }
        int d;
        if(!int.TryParse(this.scoreInput.Text.ToString(), out d))
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid score value", "OK").ToString();
            return false;
        }
        int u;
        if(!int.TryParse(this.userIdInput.Text.ToString(), out u))
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid user id", "OK").ToString();
            return false;
        }
        if(this.userRepo.GetById(u) == null)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid user id", "OK").ToString();
            return false;
        }
        if(d < 1 || d > 10)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid score value", "OK").ToString();
            return false;
        }
        int i;
        if(!int.TryParse(this.movieIdInput.Text.ToString(), out i))
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid movie id", "OK").ToString();
            return false;
        }
        if(this.movieRepo.GetById(i) == null)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid movie id", "OK").ToString();
            return false;
        }
        return true;
    }
}