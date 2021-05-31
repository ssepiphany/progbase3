using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class MoviesWindow : Window
{
    protected string title = "Movie DB";
    protected ListView allMoviesListView;
    protected Label emptyListLbl;
    protected int page = 1;
    protected MovieRepository repo;
    protected Label totalPagesLbl;
    protected Label pageLbl;
    protected Button prevPageBtn;
    protected Button newxtPageBtn;
    protected int pageLength = 12;
    protected NStack.ustring[] options;
    // private Label chooseEntityLbl;
    protected RadioGroup typeGroup; 
    public int selectedItem;
    protected UserRepository userRepository;
    protected ReviewRepository reviewRepository;
    protected ActorRepository actorRepository;
    protected User currentUser;
    protected Label currentUsername;
    protected Button createNewMovie;
    protected MenuBar menu;
    protected FrameView frameView;
    public MoviesWindow()
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
        typeGroup.SelectedItem = 0;
        typeGroup.SelectedItemChanged += OnSelectedItemChanged; 

        this.Add(typeGroup);
        

        allMoviesListView = new ListView(new List<Movie>())
        {
            Width = Dim.Fill(), Height = Dim.Fill(),
        };
        allMoviesListView.OpenSelectedItem += OnOpenMovie;;

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
        newxtPageBtn = new Button("Next")
        {
            X = Pos.Right(totalPagesLbl) + 2, Y = Pos.Top(prevPageBtn), Visible = true, 
        };
        newxtPageBtn.Clicked += OnNextPage;
        this.Add(prevPageBtn, pageLbl, totalPagesLbl, newxtPageBtn);

        emptyListLbl = new Label()
        {
            Text = "There are no movies!", Width = Dim.Fill(), Height = Dim.Fill(), X = 2, Y = 2,
        };

        frameView = new FrameView("Movies")
        {
            X = 2, Y = 8, Width = Dim.Fill() - 5, Height =  Dim.Fill() - 5,
        };
        frameView.Add(allMoviesListView);
        frameView.Add(emptyListLbl);
        this.Add(frameView);

        createNewMovie = new Button(2, 4, "Create new movie");
        createNewMovie.Clicked += OnCreateButtonClicked;
        this.Add(createNewMovie);

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

    public void SetRepositories(MovieRepository movieRepository, UserRepository userRepository, ReviewRepository reviewRepository, ActorRepository actorRepository)
    {
        this.repo = movieRepository;
        this.userRepository = userRepository;
        this.reviewRepository = reviewRepository;
        this.actorRepository = actorRepository;
    }

    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
        this.currentUsername.Text = user.login;
        this.ShowCurrentPage();
    }

    protected void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        Application.RequestStop();
    }

    protected void OnQuit()
    {
        this.selectedItem = -1;
        Application.RequestStop();
    }


    protected virtual void OnNextPage()
    {
        int totalPages = repo.GetTotalPages(pageLength);
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
        bool isEmptyList = (this.repo.GetCount() == 0);
        int totalPages = repo.GetTotalPages(pageLength);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = repo.GetTotalPages(pageLength).ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allMoviesListView.Visible = !isEmptyList;
        this.allMoviesListView.SetSource(repo.GetPage(page, pageLength));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        newxtPageBtn.Visible = (page != totalPages) && (!isEmptyList);
        this.createNewMovie.Visible = this.currentUser.moderator;
    }

    protected virtual void OnCreateButtonClicked()
    {
        CreateMovieDialog dialog = new CreateMovieDialog();
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Movie movie = dialog.GetMovie();
            int id = repo.Insert(movie);
            movie.id = id;
            allMoviesListView.SetSource(repo.GetPage(page, pageLength));
            ShowCurrentPage();
            ProcessOpenMovie(movie);
        }
    }

    protected void OnOpenMovie(ListViewItemEventArgs args)
    {
        Movie movie = (Movie)args.Value;
        ProcessOpenMovie(movie);
    }

    protected void ProcessOpenMovie(Movie movie)
    {
        OpenMovieDialog dialog = new OpenMovieDialog();
        dialog.SetMovie(movie);
        dialog.SetRepositories(actorRepository, reviewRepository, userRepository, repo);
        dialog.SetCurrentUser(this.currentUser);

        Application.Run(dialog);

        if(dialog.deleted)
        {
            TryDeleteMovie(movie);
        }

        if(dialog.updated)
        {
            bool result = repo.Update(movie.id, dialog.GetMovie());
            if(result)
            {
                allMoviesListView.SetSource(repo.GetPage(page, pageLength));
            }
            else
            {
                MessageBox.ErrorQuery("Edit movie", "Can not edit movie", "OK");
            }
        }
    }

    protected virtual void TryDeleteMovie(Movie movie)
    {
        bool res = repo.DeleteById(movie.id);
        repo.RemoveConnectionsWithMovie(movie.id);
        reviewRepository.DeleteAllByMovieId(movie.id);
        if(res)
        {
            int pages = repo.GetTotalPages(pageLength);
            if(page > pages && page > 1)
            {
                page--;
            }
            //allMoviesListView.SetSource(repo.GetPage(page, pageLength));
            this.ShowCurrentPage();
        }
        else
        {
            MessageBox.ErrorQuery("Delete movie", "Can not delete movie", "OK");
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