using Terminal.Gui;

public class OpenReviewDialog : Dialog
{
    public bool deleted;
    public bool updated;
    public Review review;
    private string dialogTitle;
    private TextField scoreInput;
    private TextField dateInput;
    private TextField movieTitleInput;
    private TextField reviewIdInput;
    private TextField userInput;
    private MovieRepository movieRepo;
    private UserRepository userRepo;
    private User user;
    private Button editBtn;
    private Button deleteBtn;
    public OpenReviewDialog()
    {
        
        this.dialogTitle = "Open review";
        this.Title = this.dialogTitle;
        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogSubmit;

        this.AddButton(backBtn);

        int rightColumn = 20;

        Label idLbl = new Label(2, 2, "Id");
        reviewIdInput = new TextField("")
        {
            X = 20, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true
        };
        this.Add(idLbl, reviewIdInput);

        Label scoreLbl = new Label(2, 4, "Score:");
        scoreInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(scoreLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(scoreLbl, scoreInput);

        Label movieTitleLbl = new Label(2, 6, "Movie title:");
        movieTitleInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(movieTitleLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(movieTitleLbl, movieTitleInput);      

        Label createdAtLbl = new Label(2, 8, "Created at:");

        dateInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(createdAtLbl),Width = 40, ReadOnly = true,
        };

        this.Add(createdAtLbl, dateInput);

        Label userLbl = new Label(2, 10, "User:");
        userInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(userLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(userLbl, userInput); 

        
        editBtn = new Button(2, 22, "Edit");
        editBtn.Clicked += OnReviewEdit;
        this.Add(editBtn);

        deleteBtn = new Button("Delete")
        {
            X = Pos.Right(editBtn) + 2, Y = Pos.Top(editBtn), 
        };
        deleteBtn.Clicked += OnAReviewDelete;
        this.Add(deleteBtn);
    }

    private void OnAReviewDelete()
    {
        int index = MessageBox.Query("Delete review", "Are you sure", "No", "Yes");
        if(index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }

    private void OnReviewEdit()
    {
        EditReviewDialog dialog = new EditReviewDialog();
        dialog.SetRepositories(ref movieRepo, ref userRepo);
        dialog.SetReview(this.review);
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Review updatedReview = dialog.GetReview();
            updatedReview.userId = this.user.id;
            this.updated = true;
            updatedReview.id = review.id;
            this.SetReview(updatedReview);
        }
    }

    public void SetRepositories(ref MovieRepository movieRepo, ref UserRepository userRepo)
    {
        this.movieRepo = movieRepo;
        this.userRepo = userRepo;
    }

    public void SetUser(User user)
    {
        this.user = user ;
        this.editBtn.Visible = (this.review.userId == user.id);
        this.deleteBtn.Visible = (this.review.userId == user.id || this.user.moderator);

    }

    public void SetReview(Review review)
    {
        this.review = review;
        this.reviewIdInput.Text = review.id.ToString();
        this.scoreInput.Text = review.value.ToString();
        this.movieTitleInput.Text = movieRepo.GetById(review.movieId).title;
        this.dateInput.Text = review.createdAt.ToString("F");
        this.userInput.Text = userRepo.GetById(review.userId).login;
    }

    private void OnOpenDialogSubmit()
    {
        Application.RequestStop();
    }
}