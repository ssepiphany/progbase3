using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class ReviewsWindow : Window
{
    public string title = "My reviews";
    protected ListView allReviewsListView;
    protected Label emptyListLbl;
    protected int page = 1;
    protected Label totalPagesLbl;
    protected Label pageLbl;
    protected Button prevPageBtn;
    protected Button nextPageBtn;
    protected int pageLength = 12;
    protected NStack.ustring[] options;
    protected RadioGroup typeGroup; 
    public int selectedItem;
    protected MovieRepository movieRepository;
    protected UserRepository userRepository;
    protected ReviewRepository reviewRepository;
    protected Label currentUsername;
    protected User currentUser;
    protected Button createNewReview;
    protected FrameView frameView;
    protected string searchValue = "";
    protected TextField searchInput;
    public ReviewsWindow()
    {
        this.Title = this.title;

        options = new NStack.ustring[]{"movies", "actors", "users", "my reviews"};
        typeGroup = new RadioGroup()
        {
            X = Pos.AnchorEnd() - 15, Y = 2, RadioLabels = options, 
        };
        typeGroup.SelectedItem = 3;
        typeGroup.SelectedItemChanged += OnSelectedItemChanged; 

        this.Add(typeGroup);

        allReviewsListView = new ListView(new List<Review>())
        {
            Width = Dim.Fill(), Height = Dim.Fill(),
        };
        allReviewsListView.OpenSelectedItem += OnOpenReview;

        prevPageBtn = new Button(2, 6, "Prev");
        prevPageBtn.Clicked += OnPreviousPage;
        pageLbl = new Label("?")
        {
            X = Pos.Right(prevPageBtn) + 2, Y = Pos.Top(prevPageBtn), Width = 5,
        };
        totalPagesLbl = new Label("?")
        {
            X = Pos.Right(pageLbl) + 2, Y = Pos.Top(prevPageBtn), Width = 5,
        };
        nextPageBtn = new Button("Next")
        {
            X = Pos.Right(totalPagesLbl) + 2, Y = Pos.Top(prevPageBtn), Visible = true, 
        };
        nextPageBtn.Clicked += OnNextPage;
        this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);

        emptyListLbl = new Label()
        {
            Text = "There are no reviews!", Width = Dim.Fill(), Height = Dim.Fill(), X = 2, Y = 2,
        };

        frameView = new FrameView("Reviews")
        {
            X = 2, Y = 8, Width = Dim.Fill() - 5, Height =  Dim.Fill() - 5,
        };
        frameView.Add(allReviewsListView);
        frameView.Add(emptyListLbl);
        this.Add(frameView);


        createNewReview = new Button(2, 2, "Add new review");
        createNewReview.Clicked += OnCreateButtonClicked;
        this.Add(createNewReview);

        Label currentUserLbl = new Label("Current user:")
        {
            X = Pos.Percent(5), Y = Pos.Percent(95), Width = 15,
        };

        currentUsername = new Label("?")
        {
            X = Pos.Right(currentUserLbl) + 1, Y = Pos.Top(currentUserLbl), Width = 20, 
        };

        this.Add(currentUserLbl, currentUsername);

        searchInput = new TextField(2, 4, 20, "");
        searchInput.KeyPress += OnSearchEnter;
        this.Add(searchInput);
    }

    protected void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        Application.RequestStop();
    }

    protected void OnSearchEnter(KeyEventEventArgs args)
    {
        if (args.KeyEvent.Key == Key.Enter)
        {
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPage();
        }
    }

    public string GetWindowTitle()
    {
        return this.title;
    }


    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
        this.currentUsername.Text = currentUser.login;
        this.ShowCurrentPage();
    }

    public void SetRepositories(MovieRepository movieRepository, UserRepository userRepository, ReviewRepository reviewRepository)
    {
        this.movieRepository = movieRepository;
        this.userRepository = userRepository;
        this.reviewRepository = reviewRepository;
    }

    protected virtual void OnNextPage()
    {
        int totalPages = reviewRepository.GetTotalPagesForAuthor(pageLength, currentUser.id);
        if(page >= totalPages)
        {
            return;
        }
        this.page += 1;
        this.ShowCurrentPage();
    }

    protected void OnPreviousPage()
    {
        if(page == 1)
        {
            return;
        }
        this.page -= 1;
        this.ShowCurrentPage();
    }

    protected virtual void ShowCurrentPage()
    {
        int totalPages = reviewRepository.GetSearchPagesCount(searchValue, this.currentUser.id, pageLength);
        if(page > totalPages && page > 1)
        {
            page = totalPages;
        }
        bool isEmptyList = (totalPages == 0);
        this.searchInput.Visible = !(this.reviewRepository.GetTotalPagesForAuthor(pageLength, this.currentUser.id) == 0);
        
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allReviewsListView.Visible = !isEmptyList;
        this.allReviewsListView.SetSource(reviewRepository.GetSearchPage(this.currentUser.id, searchValue, page, pageLength));
        
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        nextPageBtn.Visible = (page != totalPages) && (!isEmptyList);
    }

    protected virtual void OnCreateButtonClicked()
    {
        CreateReviewDialog createReviewDialog = new CreateReviewDialog();
        createReviewDialog.SetRepositories(ref movieRepository, ref userRepository);
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

    protected void OnOpenReview(ListViewItemEventArgs args)
    {
        Review review = (Review)args.Value;
        ProcessOpenReview(review);
    }

    protected void ProcessOpenReview(Review review)
    {
        OpenReviewDialog dialog = new OpenReviewDialog();
        dialog.SetRepositories(ref movieRepository, ref userRepository);
        dialog.SetReview(review);
        dialog.SetUser(this.currentUser);

        Application.Run(dialog);

        if(dialog.deleted)
        {
            TryDeleteReview(review);
        }

        if(dialog.updated)
        {
            dialog.review.userId = this.currentUser.id;
            bool result = reviewRepository.Update(review.id, dialog.review);
            if(result)
            {
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Edit review", "Can not edit review", "OK");
            }
        }
    }

    protected virtual void TryDeleteReview(Review review)
    {
        bool result = reviewRepository.DeleteById(review.id);
        if(result)
        {
            int pages = reviewRepository.GetTotalPagesForAuthor(pageLength, this.currentUser.id);
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
}