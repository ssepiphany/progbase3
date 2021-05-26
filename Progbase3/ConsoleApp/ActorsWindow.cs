using Terminal.Gui;
using System.Collections.Generic;
using System.IO;

public class ActorsWindow : Window
{
    
    public string title = "Actor DB";
    private ListView allActorsListView;
    private Label emptyListLbl;
    private int page = 1;
    private Label totalPagesLbl;
    private Label pageLbl;
    private Button prevPageBtn;
    private Button newxtPageBtn;
    private int pageLength = 12;
    private NStack.ustring[] options;
    private RadioGroup typeGroup; 
    public int selectedItem;
    private MovieRepository movieRepository;
    private UserRepository userRepository;
    private ActorRepository actorRepository;
    private ReviewRepository reviewRepository;
    public ActorsWindow()
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

        FrameView frameView = new FrameView("Actors")
        {
            X = 2, Y = 8, Width = Dim.Fill() - 1, Height =  Dim.Fill() - 3,
        };
        frameView.Add(allActorsListView);
        frameView.Add(emptyListLbl);
        this.Add(frameView);


        Button createNewActor = new Button(2, 4, "Create new actor");
        createNewActor.Clicked += OnCreateButtonClicked;
        this.Add(createNewActor);

        // Button backBtn = new Button()
        // {
        //     X = Pos.Center(), Y = Pos.Percent(98), Text = "Back",
        // };
        // backBtn.Clicked += OnQuit;
        // this.Add(backBtn);
    }

    private void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        Application.RequestStop();
    }

    public void SetRepositories(MovieRepository movieRepository, UserRepository userRepository, ActorRepository actorRepository, ReviewRepository reviewRepository)
    {
        this.movieRepository = movieRepository;
        this.userRepository = userRepository;
        this.actorRepository = actorRepository;
        this.reviewRepository = reviewRepository;
        this.ShowCurrentPage();
    }

    private void OnQuit()
    {
        this.selectedItem = -1;
        Application.RequestStop();
    }


    private void OnNextPage()
    {
        int totalPages = actorRepository.GetTotalPages(pageLength);
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
        bool isEmptyList = (this.actorRepository.GetCount() == 0);
        int totalPages = actorRepository.GetTotalPages(pageLength);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = actorRepository.GetTotalPages(pageLength).ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allActorsListView.Visible = !isEmptyList;
        this.allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        newxtPageBtn.Visible = (page != totalPages) && (!isEmptyList);
    }

    private void OnCreateButtonClicked()
    {
        CreateActorDialog dialog = new CreateActorDialog();
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Actor actor = dialog.GetActor();
            int id = actorRepository.Insert(actor);
            actor.id = id;
            allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
            ShowCurrentPage();
            ProcessOpenActor(actor);
        }
    }

    private void OnOpenActor(ListViewItemEventArgs args)
    {
        Actor actor = (Actor)args.Value;
        ProcessOpenActor(actor);
    }

    private void ProcessOpenActor(Actor actor)
    {
        OpenActorDialog dialog = new OpenActorDialog();
        dialog.SetActor(actor);

        Application.Run(dialog);

        if(dialog.deleted)
        {
            bool result = actorRepository.DeleteById(actor.id);
            if(result)
            {
                int pages = actorRepository.GetTotalPages(pageLength);
                if(page > pages && page > 1)
                {
                    page--;
                }
                allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
                this.ShowCurrentPage();
            }
            else
            {
                MessageBox.ErrorQuery("Delete actor", "Can not delete actor", "OK");
            }
        }

        if(dialog.updated)
        {
            bool result = actorRepository.Update(actor.id, dialog.GetActor());
            if(result)
            {
                allActorsListView.SetSource(actorRepository.GetPage(page, pageLength));
            }
            else
            {
                MessageBox.ErrorQuery("Edit actor", "Can not edit actor", "OK");
            }
        }
    }

    private void OnExport()
    {
        ExportDialog exportDialog = new ExportDialog();
        exportDialog.SetRepositories(userRepository, reviewRepository);

        Application.Run(exportDialog);

        if(!exportDialog.canceled)
        {
            ExportImport.Export(exportDialog.user, exportDialog.dirPathInput.Text.ToString());
        }
    }

    private void OnImport()
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