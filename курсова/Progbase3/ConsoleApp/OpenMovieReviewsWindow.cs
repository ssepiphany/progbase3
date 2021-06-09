using Terminal.Gui;

public class OpenMovieReviewsWindow : ReviewsWindow
{
    protected Movie movie;
    public OpenMovieReviewsWindow()
    {
        this.title = "View reviews";
        this.Title = this.title;

       createNewReview.Text = "Add new review";
       typeGroup.Visible = false;
       this.searchInput.Visible= false;

        Button backBtn = new Button()
        {
            X = Pos.Center(), Y = Pos.Percent(98), Text = "Back",
        };
        backBtn.Clicked += OnQuit;
        this.Add(backBtn);
    }

    protected void OnQuit()
    {
        this.selectedItem = -1;
        Application.RequestStop();
    }

    public void SetMovie(Movie movie)
    {
        this.movie = movie;
        this.frameView.Title = $"\"{movie.title}\" reviews";
    }

    protected override void OnNextPage()
    {
        int totalPages = reviewRepository.GetTotalPagesForMovie(pageLength, this.movie.id);
        if(page >= totalPages)
        {
            return;
        }
        this.page += 1;
        this.ShowCurrentPage();
    }

    protected override void ShowCurrentPage()
    {
        bool isEmptyList = (this.reviewRepository.GetCountForMovie(this.movie.id) == 0);
        int totalPages = reviewRepository.GetTotalPagesForMovie(pageLength, this.movie.id);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allReviewsListView.Visible = !isEmptyList;
        this.allReviewsListView.SetSource(reviewRepository.GetPageForMovie(page, pageLength, this.movie.id));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        nextPageBtn.Visible = (page != totalPages) && (!isEmptyList);
    }

    protected override void TryDeleteReview(Review review)
    {
        bool result = reviewRepository.DeleteById(review.id);
        if(result)
        {
            int pages = reviewRepository.GetTotalPagesForMovie(pageLength, movie.id);
            if(page > pages && page > 1)
            {
                page--;
            }
            this.ShowCurrentPage();
        }
        else
        {
            MessageBox.ErrorQuery("Delete review", "Can not delete review", "OK");
        }
    }

    protected override void OnCreateButtonClicked()
    {
        CreateReviewDialog createReviewDialog = new CreateReviewDialog();
        createReviewDialog.SetRepositories(ref movieRepository, ref userRepository);
        createReviewDialog.SetMovie(this.movie);
        Application.Run(createReviewDialog);

        if(!createReviewDialog.canceled)
        {
            Review review = createReviewDialog.GetReview();
            review.userId = this.currentUser.id;
            int id = reviewRepository.Insert(review);
            review.id = id;
            ShowCurrentPage();
            ProcessOpenReview(review);
        }
    }
}