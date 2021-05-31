using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class ActorsWindow : Window
{
    
    protected string title = "Actor DB";
    protected ListView allActorsListView;
    protected Label emptyListLbl;
    protected int page = 1;
    protected Label totalPagesLbl;
    protected Label pageLbl;
    protected Button prevPageBtn;
    protected Button newxtPageBtn;
    protected int pageLength = 12;
    protected NStack.ustring[] options;
    protected RadioGroup typeGroup; 
    public int selectedItem;
    protected UserRepository userRepository;
    protected ActorRepository actorRepository;
    protected ReviewRepository reviewRepository;
    protected MovieRepository movieRepository;
    protected User currentUser;
    protected Label currentUsername;
    protected Button createNewActor;
    protected MenuBar menu;
    protected FrameView frameView;
    public ActorsWindow()
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
        typeGroup.SelectedItem = 1;
        typeGroup.SelectedItemChanged += OnSelectedItemChanged; 

        this.Add(typeGroup);

        allActorsListView = new ListView(new List<Actor>())
        {
            Width = Dim.Fill(), Height = Dim.Fill(),
        };
        allActorsListView.OpenSelectedItem += OnOpenActor;;

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
            Text = "There are no actors!", Width = Dim.Fill(), Height = Dim.Fill(), X = 2, Y = 2,
        };

        frameView = new FrameView("Actors")
        {
            X = 2, Y = 8, Width = Dim.Fill() - 5, Height =  Dim.Fill() - 5,
        };
        frameView.Add(allActorsListView);
        frameView.Add(emptyListLbl);
        this.Add(frameView);

        createNewActor = new Button(2, 4, "Create new actor");
        createNewActor.Clicked += OnCreateButtonClicked;
        this.Add(createNewActor);

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
        this.currentUsername.Text = user.login;
        this.ShowCurrentPage();
    }
    public void SetRepositories(UserRepository userRepository, ActorRepository actorRepository, ReviewRepository reviewRepository, MovieRepository movieRepository)
    {
        this.userRepository = userRepository;
        this.actorRepository = actorRepository;
        this.reviewRepository = reviewRepository;
        this.movieRepository = movieRepository;
    }

    protected void OnQuit()
    {
        this.selectedItem = -1;
        Application.RequestStop();
    }

    protected virtual void OnNextPage()
    {
        int totalPages = actorRepository.GetTotalPages(pageLength);
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
        bool isEmptyList = (this.actorRepository.GetCount() == 0);
        int totalPages = actorRepository.GetTotalPages(pageLength);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allActorsListView.Visible = !isEmptyList;
        this.allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        newxtPageBtn.Visible = (page != totalPages) && (!isEmptyList);
        this.createNewActor.Visible = this.currentUser.moderator;
    }

    protected virtual void OnCreateButtonClicked()
    {
        CreateActorDialog dialog = new CreateActorDialog();
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Actor actor = dialog.GetActor();
            int id = actorRepository.Insert(actor);
            actor.id = id;
            // allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
            ShowCurrentPage();
            ProcessOpenActor(actor);
        }
    }

    protected void OnOpenActor(ListViewItemEventArgs args)
    {
        Actor actor = (Actor)args.Value;
        ProcessOpenActor(actor);
    }

    protected void ProcessOpenActor(Actor actor)
    {
        OpenActorDialog dialog = new OpenActorDialog();
        dialog.SetRepositories(movieRepository, actorRepository);
        dialog.SetActor(actor);
        dialog.SetCurrentUser(this.currentUser);

        Application.Run(dialog);

        if(dialog.deleted)
        {
            TryDeleteActor(actor);
        }

        if(dialog.updated)
        {
            bool result = actorRepository.Update(actor.id, dialog.GetActor());
            if(result)
            {
                this.ShowCurrentPage();
                //allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
            }
            else
            {
                MessageBox.ErrorQuery("Edit actor", "Can not edit actor", "OK");
            }
        }
    }

    protected virtual void TryDeleteActor(Actor actor)
    {
        bool result = actorRepository.DeleteById(actor.id);
        actorRepository.RemoveConnectionsWithActor(actor.id);
        if(result)
        {
            int pages = actorRepository.GetTotalPages(pageLength);
            if(page > pages && page > 1)
            {
                page--;
            }
            //allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
            this.ShowCurrentPage();
        }
        else
        {
            MessageBox.ErrorQuery("Delete actor", "Can not delete actor", "OK");
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