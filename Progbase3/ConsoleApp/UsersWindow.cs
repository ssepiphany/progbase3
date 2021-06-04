using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class UsersWindow : Window
{
    private string title = "Users";
    private ListView allUsersListView;
    private Label emptyListLbl;
    private int page = 1;
    private UserRepository repo;
    private Label totalPagesLbl;
    private Label pageLbl;
    private Button prevPageBtn;
    private Button nextPageBtn;
    private int pageLength = 12;
    private NStack.ustring[] options;
    private RadioGroup typeGroup; 
    public int selectedItem;
    private ReviewRepository reviewRepository;
    private User currentUser;
    private Label currentUsername;
    private Button createNewUser; 
    protected string searchValue = "";
    protected TextField searchInput;
    public UsersWindow()
    {
        this.Title = this.title;

        options = new NStack.ustring[]{"movies", "actors", "users", "my reviews"};
        typeGroup = new RadioGroup()
        {
            X = Pos.AnchorEnd() - 15, Y = 2, RadioLabels = options, 
        };
        typeGroup.SelectedItem = 2;
        typeGroup.SelectedItemChanged += OnSelectedItemChanged; 

        this.Add(typeGroup);

        allUsersListView = new ListView(new List<User>())
        {
            Width = Dim.Fill(), Height = Dim.Fill(),
        };
        allUsersListView.OpenSelectedItem += OnOpenUser;

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
            Text = "There are no users!", Width = Dim.Fill(), Height = Dim.Fill(), X = 2, Y = 2,
        };

        FrameView frameView = new FrameView("Users")
        {
            X = 2, Y = 8, Width = Dim.Fill() - 5, Height =  Dim.Fill() - 5,
        };
        frameView.Add(allUsersListView);
        frameView.Add(emptyListLbl);
        this.Add(frameView);


        createNewUser = new Button(2, 2, "Create new user");
        createNewUser.Clicked += OnCreateButtonClicked;
        this.Add(createNewUser);

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

    public void SetRepositories(UserRepository userRepository, ReviewRepository reviewRepository)
    {
        this.repo = userRepository;
        this.reviewRepository = reviewRepository;
    }

    public string GetWindowTitle()
    {
        return this.title;
    }

    protected void OnSearchEnter(KeyEventEventArgs args)
    {
        if (args.KeyEvent.Key == Key.Enter)
        {
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPage();
        }
    }

    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
        this.currentUsername.Text = user.login;
        this.ShowCurrentPage();
    }

    private void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        Application.RequestStop();
    }


    private void OnNextPage()
    {
        int totalPages = repo.GetTotalPages(pageLength);
        if(page >= totalPages)
        {
            return;
        }
        this.page += 1;
        this.ShowCurrentPage();
    }

    private void OnPreviousPage()
    {
        if(page == 1)
        {
            return;
        }
        this.page -= 1;
        this.ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        int totalPages = repo.GetSearchPagesCount(searchValue, pageLength);
        if(page > totalPages && page > 1)
        {
            page = totalPages;
        }
        bool isEmptyList = (totalPages == 0);
        this.searchInput.Visible = !(this.repo.GetTotalPages(pageLength) == 0);
       
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allUsersListView.Visible = !isEmptyList;
        this.allUsersListView.SetSource(repo.GetSearchPage(searchValue, page, pageLength));
        
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        nextPageBtn.Visible = (page != totalPages) && (!isEmptyList);
        this.createNewUser.Visible = this.currentUser.moderator;
    }

    private void OnCreateButtonClicked()
    {
        CreateUserDialog createUserDialog = new CreateUserDialog();
        createUserDialog.SetRepository(repo);
        Application.Run(createUserDialog);

        if(!createUserDialog.canceled)
        {
            User user = createUserDialog.GetUser();
            bool registered = Authentication.RegisterUser(user, repo);
            if(!registered)
            {
                this.Title = MessageBox.ErrorQuery("Error", $"Username \"{user.login}\" already exists", "OK").ToString();
                this.Title = title;
                ShowCurrentPage();
            }
            else
            {
                ShowCurrentPage();
                ProcessOpenUser(user);
            } 
        }
    }

    private void OnOpenUser(ListViewItemEventArgs args)
    {
        User user = (User)args.Value;
        ProcessOpenUser(user);
    }

    private void ProcessOpenUser(User user)
    {
        OpenUserDialog dialog = new OpenUserDialog();
        dialog.SetRepository(repo);
        dialog.SetUser(user);
        dialog.SetCurrentUser(this.currentUser);

        Application.Run(dialog);

        if(dialog.deleted)
        {
            bool result = repo.DeleteById(user.id);
            bool result2 = reviewRepository.DeleteAllByAuthorId(user.id);
            if(result && result2)
            {
                int pages = repo.GetTotalPages(pageLength);
                if(page > pages && page > 1)
                {
                    page--;
                }
                allUsersListView.SetSource(repo.GetPage(page, pageLength));
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Delete user", "Can not delete user", "OK");
            }
        }

        if(dialog.updated)
        {
            User user2 = dialog.GetUser();
            user2.password = Authentication.HashPassword(user2.password);
            bool result = repo.Update(user.id, user2);
            if(result)
            {
                allUsersListView.SetSource(repo.GetPage(page, pageLength));
            }
            else
            {
                MessageBox.ErrorQuery("Edit user", "Can not edit user", "OK");
            }
        }
    }
}