using System.IO;
using Terminal.Gui;

public class WelcomeWindow : Window
{
    private TextField usernameInput;
    private TextField passwordInput;
    private string title = "Welcome";
    private UserRepository userRepo;
    public bool wantRegistration;
    public bool authorized;
    private User user;
    public WelcomeWindow()
    {
        MenuBar menu = new MenuBar
        (new MenuBarItem[] 
        {
           new MenuBarItem ("_File", new MenuItem [] 
           {
               new MenuItem ("_Exit", "", OnQuit),
           }),
           new MenuBarItem ("_Help", new MenuItem [] 
           {
               new MenuItem ("_About", "", OnAbout)
           }),
        });
       this.Add(menu);

        this.Title = title;

        Label loginLbl = new Label("Log in")
        {
            X = Pos.Center() - 3, Y = Pos.Center()-8, Width = 10, 
        };

        this.Add(loginLbl);

        Label usernameLbl = new Label("Username")
        {
            X = Pos.Center() - 16, Y = Pos.Top(loginLbl) + 3, Width = 15,
        };
        usernameInput = new TextField("")
        {
            X = Pos.Right(usernameLbl), Y = Pos.Top(usernameLbl), Width = 20,
        };
        this.Add(usernameLbl, usernameInput);

        Label passwordLbl = new Label("Password")
        {
            X = Pos.Center() - 16, Y = Pos.Top(usernameInput) + 4, Width = 15, 
        };

        passwordInput = new TextField("")
        {
            X = Pos.Right(passwordLbl), Y = Pos.Top(passwordLbl), Width = 20, Secret = true, 
        };
        this.Add(passwordLbl, passwordInput);

        Label questionLbl = new Label("Do not have an account yet?")
        {
            X = Pos.Center() - 16, Y = Pos.Top(passwordInput) + 3, Width = 30,
        };

        Button createUserBtn = new Button("Register now")
        {
            X = Pos.Right(questionLbl) - 12, Y = Pos.Top(passwordInput) + 5, Width = 16,
        };
        createUserBtn.Clicked += OnRegister;

        this.Add(questionLbl, createUserBtn);

        Button submitBtn = new Button("Submit")
        {
            X = Pos.Center() -4, Y = Pos.Top(createUserBtn) + 2, Width = 10,
        };
        submitBtn.Clicked += OnSubmit;

        this.Add(submitBtn);
    }

    public void SetRepository(UserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    public User GetUser()
    {
        return this.user;
    }

    private void OnQuit()
    {
        Application.RequestStop();
    }

    private void OnRegister()
    {
        this.wantRegistration = true;
        Application.RequestStop();
    }

    private void OnSubmit()
    {
        if(!ValidateInput())
        {
            this.Title = this.title ;
            return;
        }
        this.authorized = true;
        Application.RequestStop();
    }

    private bool ValidateInput()
    {
        if(usernameInput.Text.IsEmpty || passwordInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all the fields", "OK").ToString();
            return false;
        }
        User currentUser = Authentication.LoginUser(usernameInput.Text.ToString(), passwordInput.Text.ToString(), userRepo);
        if(currentUser == null)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid username or password", "OK").ToString();
            return false;
        }
        this.user = currentUser;
        return true;
    }

    private void OnAbout()
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