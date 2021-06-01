using System;
using System.Data;
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
            // string dbPath = "C:/Users/Sofia/projects/progbase3/data/database.db";
            string dbPath = "../../data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            ConnectionState state = connection.State;

            if(state == ConnectionState.Open)
            {
                ExecuteProgram(connection);
            }
            else
            {
                Application.Init();
                Toplevel top = Application.Top;
                Window window = new Window("Movie DB");
                MessageBox.ErrorQuery("Error", "Cannot connect to database", "OK");
                Button exitBtn = new Button()
                {
                    X = Pos.Center(), Y = Pos.Center(), Text = "Exit", Width = 8, 
                };
                exitBtn.Clicked += OnExit;
                window.Add(exitBtn);
                top.Add(window);
                Application.Run();
            }

             connection.Close();
        }

        static void OnExit()
        {
            Application.RequestStop();
        }

        static void SetDotSeparator()
        {
            CultureInfo englishUSCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = englishUSCulture;
        }

        static void ExecuteProgram(SqliteConnection connection)
        {
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


            MyMenu menu = new MyMenu();
            menu.SetCurrentUser(currentUser);
            menu.SetRepositories(movieRepo, userRepo, actorRepo, reviewRepo);
            while(true)
            {
                if(selectedItem == 0)
                {
                    MoviesWindow win = new MoviesWindow();
                    menu.SetWindow(win, win.GetWindowTitle());
                    win.SetRepositories(movieRepo, userRepo, reviewRepo, actorRepo);
                    win.SetCurrentUser(currentUser);

                    top.Add(menu, win);
                    Application.Run();
                    if (menu.GetSelectedItem() == -1)
                    {
                        selectedItem = menu.GetSelectedItem();
                    }
                    else selectedItem = win.selectedItem;
                }
                if(selectedItem == 1)
                {
                    ActorsWindow win = new ActorsWindow();
                    menu.SetWindow(win, win.GetWindowTitle());
                    win.SetRepositories(userRepo, actorRepo, reviewRepo, movieRepo);
                    win.SetCurrentUser(currentUser);
                    top.Add(menu, win);
                    Application.Run();
                    if (menu.GetSelectedItem() == -1)
                    {
                        selectedItem = menu.GetSelectedItem();
                    }
                    else selectedItem = win.selectedItem;
                }
                if(selectedItem == 3)
                {
                    ReviewsWindow win = new ReviewsWindow();
                    menu.SetWindow(win, win.GetWindowTitle());
                    win.SetRepositories(movieRepo, userRepo, reviewRepo);
                    win.SetCurrentUser(currentUser);
                    top.Add(menu, win);
                    Application.Run();
                    if (menu.GetSelectedItem() == -1)
                    {
                        selectedItem = menu.GetSelectedItem();
                    }
                    else selectedItem = win.selectedItem;
                }
                if(selectedItem == 2)
                {
                    UsersWindow win = new UsersWindow();
                    menu.SetWindow(win, win.GetWindowTitle());
                    win.SetRepositories(userRepo, reviewRepo);
                    win.SetCurrentUser(currentUser);
                    top.Add(menu, win);
                    Application.Run();
                    if (menu.GetSelectedItem() == -1)
                    {
                        selectedItem = menu.GetSelectedItem();
                    }
                    else selectedItem = win.selectedItem;
                }
                if(selectedItem == -1)
                {
                    break;
                }
            }
        }
    }
}
