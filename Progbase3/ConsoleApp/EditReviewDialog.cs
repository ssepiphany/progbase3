using Terminal.Gui;

public class EditReviewDialog : CreateReviewDialog
{
    
    private TextField idInput;
    private TextField dateText;
    public EditReviewDialog()
    {
        this.dialogTitle = "Edit review";
        this.Title = this.dialogTitle;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = 20, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true
        };
        this.Add(idLbl, idInput);

        dateInput.Visible = false;
        timeInput.Visible = false;
        dateText = new TextField()
        {
            X = 20, Y = Pos.Top(createdAtLbl), Width = 40, ReadOnly = true,
        };
        this.Add(dateText);
    }

    public void SetReview(Review review)
    {
        this.idInput.Text = review.id.ToString();
        this.scoreInput.Text = review.value.ToString();
        this.movieTitleInput.Text = movieRepo.GetById(review.movieId).title;
        this.dateText.Text = review.createdAt.ToString("F");
    }
}