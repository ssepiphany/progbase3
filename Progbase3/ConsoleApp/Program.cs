using System;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Terminal.Gui;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SetDotSeparator();
            string dbPath = "C:/Users/Sofia/projects/progbase3/data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            MovieRepository movieRepo = new MovieRepository(connection);
            ActorRepository actorRepo = new ActorRepository(connection);
            ReviewRepository reviewRepo = new ReviewRepository(connection);
            UserRepository userRepo = new UserRepository(connection);

            int selectedItem = 0;

            while(true)
            {
                Application.Init();
                // MenuLine menu = new MenuLine();
                // menu.SetRepositories(movieRepo, userRepo, actorRepo, reviewRepo);
                Toplevel top = Application.Top;
                if(selectedItem == 0)
                {
                    MoviesWindow win = new MoviesWindow();
                    win.SetRepositories(movieRepo, userRepo, actorRepo, reviewRepo);

                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                else if(selectedItem == 1)
                {
                    ActorsWindow win = new ActorsWindow();
                    win.SetRepositories(movieRepo, userRepo, actorRepo, reviewRepo);
                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                else if(selectedItem == 2)
                {
                    UsersWindow win = new UsersWindow();
                    win.SetRepository(userRepo);
                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                else if(selectedItem == 3)
                {
                    ReviewsWindow win = new ReviewsWindow();
                    win.SetRepositories(movieRepo, userRepo, actorRepo, reviewRepo);
                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                else
                {
                    break;
                }
            }
            connection.Close();

            // Console.WriteLine("Please, enter your command:");
            // string[] command = Console.ReadLine().Trim().Split();
            // UserRepository userRepo = new UserRepository(connection);
            // ReviewRepository reviewRepo = new ReviewRepository(connection);
            // User user = userRepo.GetById(7);
            // if(command[0] == "generate")
            // {
            //     // generate {movie} {5}
            //     UserInterface.ProcessGenerate(command, connection);
            // }
            // if(command[0] == "export")
            // {
            //     user = reviewRepo.GetReviewsForExport(user);
            //     ExportImport.Export(user, "C:/Users/Sofia/my");
            // }
        }

        static void OnQuit()
        {
            Application.RequestStop();
        }

        static void SetDotSeparator()
        {
            CultureInfo englishUSCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = englishUSCulture;
        }
    }
}
