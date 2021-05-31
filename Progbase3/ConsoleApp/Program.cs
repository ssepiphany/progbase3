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
            bool authorized = false;
            User currentUser = new User();

            Application.Init();
            Toplevel top = Application.Top;
            while(!authorized)
            {
                WelcomeWindow welcomeWin = new WelcomeWindow();
                welcomeWin.SetRepository(userRepo);

                top.Add(welcomeWin);
                Application.Run();
                authorized = welcomeWin.authorized;
                currentUser = welcomeWin.GetUser();

                if(welcomeWin.wantRegistration)
                {
                    RegisterWindow registrationWin = new RegisterWindow();
                    registrationWin.SetRepository(userRepo);
                    top.Add(registrationWin);
                    Application.Run();
                    if(registrationWin.wantToQuit)
                    {
                        return;
                    }
                }
                else
                {
                    if(!authorized) return;
                }
            }

            while(true)
            {
                if(selectedItem == 0)
                {
                    MoviesWindow win = new MoviesWindow();
                    win.SetRepositories(movieRepo, userRepo, reviewRepo, actorRepo);
                    win.SetCurrentUser(currentUser);

                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                if(selectedItem == 1)
                {
                    ActorsWindow win = new ActorsWindow();
                    win.SetRepositories(userRepo, actorRepo, reviewRepo, movieRepo);
                    win.SetCurrentUser(currentUser);
                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                if(selectedItem == 3)
                {
                    ReviewsWindow win = new ReviewsWindow();
                    win.SetRepositories(movieRepo, userRepo, reviewRepo);
                    win.SetCurrentUser(currentUser);
                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                if(selectedItem == 2)
                {
                    UsersWindow win = new UsersWindow();
                    win.SetRepositories(userRepo, reviewRepo);
                    win.SetCurrentUser(currentUser);
                    top.Add(win);
                    Application.Run();
                    selectedItem = win.selectedItem;
                }
                if(selectedItem == -1)
                {
                    break;
                }
            }

            connection.Close();
        }

        static void SetDotSeparator()
        {
            CultureInfo englishUSCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = englishUSCulture;
        }
    }
}
