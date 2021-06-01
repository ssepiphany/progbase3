using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
using ScottPlot;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

public class MyMenu : MenuBar
{
    private MovieRepository movieRepository;
    private UserRepository userRepository;
    private ActorRepository actorRepository;
    private ReviewRepository reviewRepository; 
    private Window win;
    private User currentUser;
    private string windowTitle; 
    private int selectedItem;
    public MyMenu()
    {
        MenuBarItem[] items = new MenuBarItem[] 
        {
           new MenuBarItem ("_File", new MenuItem [] 
           {
               new MenuItem ("_Export", "", OnExport),
               new MenuItem ("_Import", "", OnImport),
               new MenuItem ("_Generate report", "", OnGenerateReport),
               new MenuItem ("_Generate histogram", "", OnGenerateHistogram),
               new MenuItem ("_Exit", "", OnQuit),
           }),
           new MenuBarItem ("_Help", new MenuItem [] 
           {
               new MenuItem ("_About", "", OnAbout)
           }),
       };
       this.Menus = items;
    }

    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
    }

    protected void OnQuit()
    {
        this.selectedItem = -1;
        Application.RequestStop();
    }

    protected void OnSubmit()
    {
        Application.RequestStop();
    }

    public int GetSelectedItem()
    {
        return this.selectedItem;
    }

   public void SetRepositories(MovieRepository movieRepository, UserRepository userRepository, ActorRepository actorRepository, ReviewRepository reviewRepository)
    {
        this.movieRepository = movieRepository;
        this.userRepository = userRepository;
        this.actorRepository = actorRepository;
        this.reviewRepository = reviewRepository;
    }

    public void SetWindow(Window window, string title)
    {
        this.win = window;
        this.windowTitle = title;
    }

    protected void OnExport()
    {
        ExportDialog exportDialog = new ExportDialog();
        exportDialog.SetRepositories(reviewRepository);
        exportDialog.SetCurrentUser(ref this.currentUser);

        Application.Run(exportDialog);

        if(!exportDialog.canceled)
        {
            ExportImport.Export(this.currentUser, exportDialog.GetDirPath());
        }
    }

    protected void OnImport()
    {
        ImportDialog importDialog = new ImportDialog();

        Application.Run(importDialog);

        if(!importDialog.canceled)
        {
            ReviewRoot root = ExportImport.Import(importDialog.GetFilePath());
            if(root == null)
            {
                win.Title = MessageBox.ErrorQuery("Error", "Something went wrong.\r\nPlease, make sure to select valid file format", "OK").ToString();
                win.Title = this.windowTitle;
                return;
            }
            if (root.userId != this.currentUser.id)
            {
                win.Title = MessageBox.ErrorQuery("Error", "You cannot import this file,\r\nbecause it contains other user's reviews", "OK").ToString();
                win.Title = this.windowTitle;
                return;
            }
            for(int i = 0; i < root.reviews.Count; i++)
            {
                root.reviews[i].imported = true;
                root.reviews[i].userId = root.userId;
                if(reviewRepository.GetById(root.reviews[i].id) == null)
                {
                    int id = reviewRepository.Insert(root.reviews[i]);
                    root.reviews[i].id = id;
                    // reviewRepository.Update(root.reviews[i].id, root.reviews[i]);
                }
                // else
                // {
                //     // reviewRepository.Update(root.reviews[i].id, root.reviews[i]);
                // }
            }
        }
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
           {"x8", (empty) ? "-": $"\"{movieRepository.GetByReviewId(highestScoreReview.id).title}\""},
           {"x9", (empty) ? "-": highestScoreReview.createdAt.ToString("F")},
            {"x10", (empty) ? "-": lowestScoreReview.id.ToString()},
           {"x11", (empty) ? "-": lowestScoreReview.value.ToString()},
           {"x12", (empty) ? "-": $"\"{movieRepository.GetByReviewId(lowestScoreReview.id).title}\""},
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
        okBtn.Clicked += OnSubmit;
        dialog.AddButton(okBtn);

        Application.Run(dialog);
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

}