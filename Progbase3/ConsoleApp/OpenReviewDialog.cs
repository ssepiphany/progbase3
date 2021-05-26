using Terminal.Gui;
using System;

public class OpenReviewDialog : Dialog
{
    public bool deleted;
    public bool updated;
    public Review review;
    private string dialogTitle;
    private TextField scoreInput;
    private TextField dateInput;
    private TextField movieIdInput;
    private TextField reviewIdInput;
    private TextField userIdInput;
    public MovieRepository movieRepo;
    public UserRepository userRepo;
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

        Label movieIdLbl = new Label(2, 6, "Movie id:");
        movieIdInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(movieIdLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(movieIdLbl, movieIdInput);      

        Label createdAtLbl = new Label(2, 8, "Created at:");

        dateInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(createdAtLbl),Width = 40, ReadOnly = true,
        };

        this.Add(createdAtLbl, dateInput);

        Label userIdLbl = new Label(2, 10, "User id:");
        userIdInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(userIdLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(userIdLbl, userIdInput); 

        
        Button editBtn = new Button(2, 22, "Edit");
        editBtn.Clicked += OnReviewEdit;
        this.Add(editBtn);

        Button deleteBtn = new Button("Delete")
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

    public void SetReview(Review review)
    {
        this.review = review;
        this.reviewIdInput.Text = review.id.ToString();
        this.scoreInput.Text = review.value.ToString();
        this.movieIdInput.Text = review.movieId.ToString();
        this.dateInput.Text = review.createdAt.ToString("F");
        this.userIdInput.Text = review.userId.ToString();
    }

    private void OnOpenDialogSubmit()
    {
        Application.RequestStop();
    }
}