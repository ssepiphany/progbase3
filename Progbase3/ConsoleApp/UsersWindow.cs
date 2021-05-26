using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class UsersWindow : Window
{
    public string title = "User DB";
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
    private MovieRepository movieRepository;
    private ActorRepository actorRepository;
    private ReviewRepository reviewRepository;
    public UsersWindow()
    {
        MenuBar menu = new MenuBar
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

        options = new NStack.ustring[]{"movies", "actors", "users", "reviews"};
        typeGroup = new RadioGroup()
        {
            X = Pos.AnchorEnd() - 10, Y = 2, RadioLabels = options, 
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
            X = 2, Y = 8, Width = Dim.Fill() - 1, Height =  Dim.Fill() - 3,
        };
        frameView.Add(allUsersListView);
        frameView.Add(emptyListLbl);
        this.Add(frameView);


        Button createNewUser = new Button(2, 4, "Create new user");
        createNewUser.Clicked += OnCreateButtonClicked;
        this.Add(createNewUser);

        // Button backBtn = new Button()
        // {
        //     X = Pos.Center(), Y = Pos.Percent(98), Text = "Back",
        // };
        // backBtn.Clicked += OnQuit;
        // this.Add(backBtn);
    }

    public void SetRepositories(MovieRepository movieRepository, UserRepository userRepository, ActorRepository actorRepository, ReviewRepository reviewRepository)
    {
        this.repo = userRepository;
        this.movieRepository = movieRepository;
        this.actorRepository = actorRepository;
        this.reviewRepository = reviewRepository;
        this.ShowCurrentPage();
    }

    private void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        Application.RequestStop();
    }

    public void SetRepository(UserRepository repo)
    {
        this.repo = repo;
        this.ShowCurrentPage();
    }

    private void OnQuit()
    {
        this.selectedItem = -1;
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
        bool isEmptyList = (this.repo.GetCount() == 0);
        int totalPages = repo.GetTotalPages(pageLength);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allUsersListView.Visible = !isEmptyList;
        this.allUsersListView.SetSource(repo.GetPage(page, pageLength));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        nextPageBtn.Visible = (page != totalPages) && (!isEmptyList);
    }

    private void OnCreateButtonClicked()
    {
        CreateUserDialog createUserDialog = new CreateUserDialog();
        Application.Run(createUserDialog);

        if(!createUserDialog.canceled)
        {
            User user = createUserDialog.GetUser();
            int id = repo.Insert(user);
            user.id = id;
            allUsersListView.SetSource(repo.GetPage(page, pageLength));
            ShowCurrentPage();
            ProcessOpenUser(user);
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
        dialog.SetUser(user);

        Application.Run(dialog);

        if(dialog.deleted)
        {
            bool result = repo.DeleteById(user.id);
            if(result)
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
                MessageBox.ErrorQuery("Delete activity", "Can not delete activity", "OK");
            }
        }

        if(dialog.updated)
        {
            bool result = repo.Update(user.id, dialog.user);
            if(result)
            {
                allUsersListView.SetSource(repo.GetPage(page, pageLength));
            }
            else
            {
                MessageBox.ErrorQuery("Edit activity", "Can not edit activity", "OK");
            }
        }
    }

    private void OnExport()
    {
        ExportDialog exportDialog = new ExportDialog();
        exportDialog.SetRepositories(repo, reviewRepository);

        Application.Run(exportDialog);

        if(!exportDialog.canceled)
        {
            ExportImport.Export(exportDialog.user, exportDialog.dirPathInput.Text.ToString());
        }
    }

    private void OnImport()
    {
        ImportDialog importDialog = new ImportDialog();
        importDialog.SetRepositories(repo, reviewRepository);

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

    private void OnAbout()
    {
        Dialog dialog = new Dialog("About");

        Label titleLbl = new Label("Activity database");
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