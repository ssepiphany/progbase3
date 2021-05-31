using Terminal.Gui;

public class OpenCastWindow : ActorsWindow
{
    private Movie movie; 
    public OpenCastWindow()
    {
        this.title = "View cast";
        this.Title = this.title;
        this.menu.Visible = false;

       createNewActor.Text = "Add new cast member";
       typeGroup.Visible = false;

        Button backBtn = new Button()
        {
            X = Pos.Center(), Y = Pos.Percent(98), Text = "Back",
        };
        backBtn.Clicked += OnQuit;
        this.Add(backBtn);
    }

    public void SetMovie(Movie movie)
    {
        this.movie = movie;
        this.frameView.Title = $"\"{movie.title}\" cast";
    }

    public void SetRepositories(ActorRepository actorRepo, MovieRepository movieRepository)
    {
        this.actorRepository = actorRepo;
        this.movieRepository = movieRepository;
    }

    protected override void OnNextPage()
    {
        int totalPages = actorRepository.GetTotalPagesForMovie(pageLength, this.movie.id);
        if(page >= totalPages)
        {
            return;
        }
        this.page += 1;
        this.ShowCurrentPage();
    }

    protected override void ShowCurrentPage()
    {
        bool isEmptyList = (this.actorRepository.GetCountForMovie(this.movie.id) == 0);
        int totalPages = actorRepository.GetTotalPagesForMovie(pageLength, this.movie.id);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allActorsListView.Visible = !isEmptyList;
        this.allActorsListView.SetSource(actorRepository.GetPageForMovie(page, pageLength, this.movie.id));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        newxtPageBtn.Visible = (page != totalPages) && (!isEmptyList);
        this.createNewActor.Visible = this.currentUser.moderator;
    }

    protected override void TryDeleteActor(Actor actor)
    {
        actorRepository.RemoveConnectionsWithActor(actor.id);
        int pages = actorRepository.GetTotalPagesForMovie(pageLength, this.movie.id);
        if(page > pages && page > 1)
        {
            page--;
        }
        this.ShowCurrentPage();

        // bool result = actorRepository.RemoveConnectionsWithActor(actor.id);
        // if(result)
        // {
        //     int pages = actorRepository.GetTotalPagesForMovie(pageLength, this.movie.id);
        //     if(page > pages && page > 1)
        //     {
        //         page--;
        //     }
        //     this.ShowCurrentPage();
        // }
        // else
        // {
        //     MessageBox.ErrorQuery("Delete actor", "Can not delete actor from list", "OK");
        // }
    }

    protected override void OnCreateButtonClicked()
    {
        AddCastDialog dialog = new AddCastDialog();
        dialog.SetRepository(actorRepository);
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Actor actor = null;
            if(dialog.selectedItem == 0)
            {
                actor = dialog.GetActor();
                int id = actorRepository.Insert(actor);
                actor.id = id;
            }
            else
            {
                string fullname = dialog.GetActorsFullname();
                actor = actorRepository.GetByFullname(fullname);
            }
            movieRepository.ConnectMovieActor(this.movie.id, actor.id);
            ShowCurrentPage();
            ProcessOpenActor(actor);
        }
    }
}