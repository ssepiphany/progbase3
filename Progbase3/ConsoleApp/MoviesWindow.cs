using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
using ScottPlot;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System;

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
    // protected MyMenu menu;
    protected FrameView frameView;
    public MoviesWindow()
    {
        // menu = new MenuBar
        // (new MenuBarItem[] 
        // {
        //    new MenuBarItem ("_File", new MenuItem [] 
        //    {
        //        new MenuItem ("_Export", "", OnExport),
        //        new MenuItem ("_Import", "", OnImport),
        //        new MenuItem ("_Generate report", "", OnGenerateReport),
        //        new MenuItem ("_Generate histogram", "", OnGenerateHistogram),
        //        new MenuItem ("_Exit", "", OnQuit),
        //    }),
        //    new MenuBarItem ("_Help", new MenuItem [] 
        //    {
        //        new MenuItem ("_About", "", OnAbout)
        //    }),
        // });
    //    this.Add(menu);

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
        // menu.SetRepositories(repo, userRepository, actorRepository, reviewRepository);
    }

    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
        this.currentUsername.Text = user.login;
        // menu.SetCurrentUser(currentUser);
        this.ShowCurrentPage();
    }

    public string GetWindowTitle()
    {
        return this.title;
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

    protected void OnGenerateHistogram()
    {
        // var plt = new ScottPlot.Plot(600, 400);

        // Random rand = new Random(0);
        // double[] values = DataGen.RandomNormal(rand, pointCount: 1000, mean: 50, stdDev: 20);
        // var hist = new ScottPlot.Statistics.Histogram(values, min: 0, max: 100);

        // double barWidth = hist.binSize * 1.2; // slightly over-side to reduce anti-alias rendering artifacts

        // plt.PlotBar(hist.bins, hist.countsFrac, barWidth: barWidth, outlineWidth: 0);
        // plt.PlotScatter(hist.bins, hist.countsFracCurve, markerSize: 0, lineWidth: 2, color: System.Drawing.Color.Black);
        // double[] curveXs = DataGen.Range(pop.minus3stDev, pop.plus3stDev, .1);
        // double[] curveYs = pop.GetDistribution(curveXs);
        // plt.PlotScatter(curveXs, curveYs, markerSize: 0, lineWidth: 2);
        // plt.Legend();
        // plt.Title("Normal Random Data");
        // plt.YLabel("Frequency (fraction)");
        // plt.XLabel("Value (units)");
        // // plt.Axis(null, null, 0, null);
        // plt.SetAxisLimits(null, null, 0, null);
        // plt.Grid(lineStyle: LineStyle.Dot);

        // plt.SaveFig("Advanced_Statistics_Histogram.png");

        var plt = new ScottPlot.Plot(600, 400);

        Dictionary<int, int> scoresFrequency =  reviewRepository.GetReviewsForHistogram(this.currentUser);
        List<Review> reviews = reviewRepository.GetAllByAuthorId(this.currentUser.id);
        double[] values = new double[reviews.Count];
        Dictionary<int, int>.KeyCollection keys = scoresFrequency.Keys;  
        int c = 0;
        // for (int i = 0; i < reviews.Count ; i++)
        // {
        //     values[i] = reviews[i].value;
        // }
        foreach (int key in keys)
        {
            values[c] = key;
            c++;
        }
        var hist = new ScottPlot.Statistics.Histogram(values, min: 0, max: 11);

        // // c = 0;


        foreach (int key in keys)
        {
            hist.countsFrac.SetValue( scoresFrequency[key],key);
        }

        
        double barWidth = hist.binSize;

        plt.PlotBar(hist.bins, hist.countsFrac, barWidth: barWidth, outlineWidth: 0);
        // plt.AddBar(hist.bins, hist.counts);
        plt.PlotScatter(hist.bins, hist.countsFracCurve, markerSize: 2, lineWidth: 2, color: System.Drawing.Color.Black);
        // plt.AddScatter(hist.bins, hist.countsFracCurve, markerSize: 0, lineWidth: 2, color: System.Drawing.Color.Black);
        plt.Title("Frequency of reviews' scores");
        plt.YLabel("Frequency (fraction)");
        plt.XLabel("Value (units)");
        // plt.Axis(null, null, 0, null);
        plt.SetAxisLimits(null, null, 0, null);
        plt.Grid(lineStyle: LineStyle.Dot);
        string imagePath = "./histogram.png";

        plt.SaveFig(imagePath);
        // // return imagePath;
    }

    protected void OnGenerateReport()
    {
        string zipPath = @"../../data/template.docx";
        string extractPath = @"../../data/template";
        ZipFile.ExtractToDirectory(zipPath, extractPath);
        GenerateReportDialog dialog = new GenerateReportDialog();
        Application.Run(dialog);
        string dirPath = dialog.GetDirPath();
        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        XElement root = XElement.Load("../../data/template/word/document.xml");
        List<Review> reviews = reviewRepository.GetAllByAuthorId(this.currentUser.id);
        bool empty = (reviews.Count == 0);
        Review highestScoreReview = null;
        Review lowestScoreReview = null;
        string imagePath = "../../data/template/word/media/image1.png";
        if(!empty)
        {
            highestScoreReview = reviewRepository.GetUserHighestScoreReview(reviews);
            lowestScoreReview = reviewRepository.GetUserLowestScoreReview(reviews);

            // string histogramPath = this.OnGenerateHistogram();
            // File.Copy(histogramPath, imagePath, true);
        }
        else
        {
            File.Delete(imagePath);
        }
       Dictionary<string, string> dict = new Dictionary<string, string>
       {
           {"x1", this.currentUser.fullname},
           {"x2", this.currentUser.login},
           {"x3", this.currentUser.createdAt.ToString("F")},
           {"x4", reviews.Count.ToString()},
           {"x5", (empty) ? "-": reviewRepository.GetUserAverageScore(this.currentUser).ToString()},
           {"x6", (empty) ? "-": highestScoreReview.id.ToString()},
           {"x7", (empty) ? "-": highestScoreReview.value.ToString()},
           {"x8", (empty) ? "-": $"\"{repo.GetByReviewId(highestScoreReview.id).title}\""},
           {"x9", (empty) ? "-": highestScoreReview.createdAt.ToString("F")},
            {"x10", (empty) ? "-": lowestScoreReview.id.ToString()},
           {"x11", (empty) ? "-": lowestScoreReview.value.ToString()},
           {"x12", (empty) ? "-": $"\"{repo.GetByReviewId(lowestScoreReview.id).title}\""},
           {"x13", (empty) ? "-": lowestScoreReview.createdAt.ToString("F")},

        };
 
      FindAndReplace(root, dict);
      root.Save("../../data/template/word/document.xml", SaveOptions.DisableFormatting);


        bool exported = false;
        int c = 2;
        string zipPath2 = dirPath + "/template(1).docx";

       while(!exported)
       {
           try
            {
                ZipFile.CreateFromDirectory("../../data/template", zipPath2);
                exported = true;
            }
            catch
            {
                zipPath2 = dirPath + $"/template({c}).docx";
                c++;
            }
       }
       Directory.Delete(extractPath, true);
    }

    // protected string FormatOutput(Review review)
    // {
    //     string[] parts = new string[4];
    //     parts[0] = "id: " + review.id;
    //     parts[1] = "score: " + review.value.ToString();
    //     parts[2] = $"movie title: \"{repo.GetByReviewId(review.id).title}\"";
    //     parts[3] = "created at: " + review.createdAt.ToString("F");
    //     string res = string.Join(System.Environment.NewLine, parts);
    //     return res;
    // }

    protected void FindAndReplace(XElement node, Dictionary<string, string> dict)
    {
        if (node.FirstNode != null
            && node.FirstNode.NodeType == XmlNodeType.Text)
        {
            string replacement;
            if (dict.TryGetValue(node.Value, out replacement))
            {
                node.Value = replacement;
            }
        }
        foreach (XElement el in node.Elements())
        {
            FindAndReplace(el, dict);
        }

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
}