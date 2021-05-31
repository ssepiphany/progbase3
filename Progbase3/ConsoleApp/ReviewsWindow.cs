using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class ReviewsWindow : Window
{
    public string title = "My review";
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
    protected MenuBar menu;
    protected Button createNewReview;
    protected FrameView frameView;
    public ReviewsWindow()
    {
        menu = new MenuBar
        (new MenuBarItem[] 
        {
           new MenuBarItem ("_File", new MenuItem [] 
           {
               new MenuItem ("_Export", "", OnExport),
               new MenuItem ("_Import", "", OnImport),
               new MenuItem ("_Exit", "", OnQuit),
           }),
           new MenuBarItem ("_Help", new MenuItem [] 
           {
               new MenuItem ("_About", "", OnAbout)
           }),
        });
       this.Add(menu);

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


        createNewReview = new Button(2, 4, "Add new review");
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
    }

    protected void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        Application.RequestStop();
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

    protected void OnQuit()
    {
        this.selectedItem = -1;
        Application.RequestStop();
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
        this.currentUser.reviews = reviewRepository.GetAllByAuthorId(this.currentUser.id);
        bool isEmptyList = ( this.currentUser.reviews.Count == 0);
        int totalPages = reviewRepository.GetTotalPagesForAuthor(pageLength, this.currentUser.id);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allReviewsListView.Visible = !isEmptyList;
        this.allReviewsListView.SetSource(reviewRepository.GetAuthorPage(page, pageLength, this.currentUser.id));
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
            //allReviewsListView.SetSource(reviewRepository.GetPage(page, pageLength));
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
                //allReviewsListView.SetSource(reviewRepository.GetPage(page, pageLength));
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

    protected void OnExport()
    {
        ExportDialog exportDialog = new ExportDialog();
        exportDialog.SetRepositories(userRepository, reviewRepository);

        Application.Run(exportDialog);

        if(!exportDialog.canceled)
        {
            ExportImport.Export(exportDialog.user, exportDialog.dirPathInput.Text.ToString());
        }
    }

    protected void OnImport()
    {
        ImportDialog importDialog = new ImportDialog();
        importDialog.SetRepositories(userRepository, reviewRepository);

        Application.Run(importDialog);

        if(!importDialog.canceled)
        {
            ReviewRoot root = ExportImport.Import(importDialog.filePathInput.Text.ToString());
            if(root == null)
            {
                this.Title = MessageBox.ErrorQuery("Error", "Something went wrong.\r\nPlease, make sure to choose valid file format", "OK").ToString();
                this.Title = this.title ;
                return;
            }
            for(int i = 0; i < root.reviews.Count; i++)
            {
                root.reviews[i].imported = true;
                root.reviews[i].userId = root.userId;
                if(reviewRepository.GetById(root.reviews[i].id) != null)
                {
                    reviewRepository.Update(root.reviews[i].id, root.reviews[i]);
                }
                else
                {
                    reviewRepository.Insert(root.reviews[i]);
                }
            }
        }
    }

    protected void OnAbout()
    {
        Dialog dialog = new Dialog("About");

        Label titleLbl = new Label("Movie database");
        dialog.Add(titleLbl);

        string info = File.ReadAllText("./about");
        TextView text = new TextView()
        {
            X = Pos.Center(), Y = Pos.Center(), Width = Dim.Percent(50), 
            Height = Dim.Percent(50), Text = info, ReadOnly = true,
        };
        dialog.Add(text);

        Button okBtn = new Button()
        {
            X = Pos.AnchorEnd(), Y = 0, Text = "OK",
        };
        okBtn.Clicked += OnQuit;
        dialog.AddButton(okBtn);

        Application.Run(dialog);
    }
}