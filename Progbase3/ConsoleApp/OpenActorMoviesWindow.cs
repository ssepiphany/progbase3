using Terminal.Gui;

public class OpenActorMoviesWindow : MoviesWindow
{
    protected Actor actor; 
    public OpenActorMoviesWindow()
    {
        this.title = "View movies";
        this.Title = this.title;
        // this.menu.Visible = false;

       createNewMovie.Text = "Add new movie";
       typeGroup.Visible = false;

        Button backBtn = new Button()
        {
            X = Pos.Center(), Y = Pos.Percent(98), Text = "Back",
        };
        backBtn.Clicked += OnQuit;
        this.Add(backBtn);
    }

    public void SetActor(Actor actor)
    {
        this.actor = actor;
        this.frameView.Title = $"{actor.fullname} movies";
    }

    public void SetRepositories(ActorRepository actorRepo, MovieRepository movieRepository)
    {
        this.actorRepository = actorRepo;
        this.repo = movieRepository;
    }

    protected override void OnNextPage()
    {
        int totalPages = repo.GetTotalPagesForActor(pageLength, this.actor.id);
        if(page >= totalPages)
        {
            return;
        }
        this.page += 1;
        this.ShowCurrentPage();
    }

    protected override void ShowCurrentPage()
    {
        bool isEmptyList = (this.repo.GetCountForActor(this.actor.id) == 0);
        int totalPages = base.repo.GetTotalPagesForActor(pageLength, this.actor.id);
        this.pageLbl.Visible = !isEmptyList;
        this.pageLbl.Text = page.ToString();
        this.totalPagesLbl.Text = totalPages.ToString();
        this.totalPagesLbl.Visible = !isEmptyList;
        this.allMoviesListView.Visible = !isEmptyList;
        this.allMoviesListView.SetSource(base.repo.GetPageForActor(page, pageLength, this.actor.id));
        this.emptyListLbl.Visible = isEmptyList;
        prevPageBtn.Visible = (page != 1) && (!isEmptyList);
        newxtPageBtn.Visible = (page != totalPages) && (!isEmptyList);
        this.createNewMovie.Visible = this.currentUser.moderator;
    }

    protected override void TryDeleteMovie(Movie movie)
    {
        repo.RemoveConnectionsWithMovie(movie.id);
        int pages = repo.GetTotalPagesForActor(pageLength, this.actor.id);
        if(page > pages && page > 1)
        {
            page--;
        }
        this.ShowCurrentPage();

        // bool result = repo.RemoveConnectionsWithMovie(movie.id);
        // if(result)
        // {
        //     int pages = repo.GetTotalPagesForActor(pageLength, this.actor.id);
        //     if(page > pages && page > 1)
        //     {
        //         page--;
        //     }
        //     this.ShowCurrentPage();
        // }
        // else
        // {
        //     MessageBox.ErrorQuery("Delete actor", "Can not delete movie from list", "OK");
        // }
    }

    protected override void OnCreateButtonClicked()
    {
        AddActorMovieDialog dialog = new AddActorMovieDialog();

        dialog.SetRepository(repo);
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Movie movie = null;
            if(dialog.selectedItem == 0)
            {
                movie = dialog.GetMovie();
                int id = repo.Insert(movie);
                movie.id = id;
            }
            else
            {
                string title = dialog.GetMovieTitle();
                movie = repo.GetByTitle(title);
            }
            repo.ConnectMovieActor(movie.id, actor.id);
            ShowCurrentPage();
            ProcessOpenMovie(movie);
        }
    }
}