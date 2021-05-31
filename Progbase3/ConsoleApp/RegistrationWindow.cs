using System;
using System.IO;
using Terminal.Gui;

public class RegisterWindow : Window
{
    private UserRepository userRepo;
    private string title = "Register";
    private TextField fullnameInput;
    private TextField usernameInput;
    private TextField passwordInput;
    private DateField dateInput;
    private TimeField timeInput;
    public bool wantToQuit;
    public RegisterWindow()
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

       this.Title = this.title;

       Button backBtn = new Button("Back")
       {
           X = Pos.Center() - 6, Y = Pos.AnchorEnd() - 1, Width = 10, 
       };
        backBtn.Clicked += OnBack;

        this.Add(backBtn);

        Button submitBtn = new Button("Submit")
        {
            X = Pos.Center() + 2, Y = Pos.AnchorEnd() - 1, Width = 10,
        };
        submitBtn.Clicked += OnSubmit;

        this.Add(submitBtn);

        int rightColumn = 40;

        Label fullnameLbl = new Label(10, 6, "Fullname");
        fullnameInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(fullnameLbl), Width = 40,
        };
        this.Add(fullnameLbl, fullnameInput);

        Label usernameTitleLbl = new Label(10, 8, "Username");
        usernameInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(usernameTitleLbl), Width = 40,
        };
        this.Add(usernameTitleLbl, usernameInput);

        Label passwordLbl = new Label(10, 10, "Password:");
        passwordInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(passwordLbl), Width = 40, Secret = true, 
        };

        this.Add(passwordLbl, passwordInput);

        Label createdAtLbl = new Label(10, 12, "Created at");
        dateInput = new DateField()
        {
            X = rightColumn, Y = Pos.Top(createdAtLbl), Date = DateTime.Now, Width = 12, IsShortFormat = false, ReadOnly = true,
        };

        timeInput = new TimeField()
        {
            X = rightColumn + 12, Y = Pos.Top(createdAtLbl), Time = DateTime.Now.TimeOfDay, Width = 28, ReadOnly = true,
        };

        this.Add(createdAtLbl, dateInput, timeInput);
    }

    public void SetRepository(UserRepository userRepo)
    {

        this.userRepo = userRepo;
    }

    private void OnSubmit()
    {
        if(!ValidateInput())
        {
            this.Title = this.title ;
            return;
        }
        Application.RequestStop();
    }

    public User GetUser()
    {
        User user = new User();
        user.fullname = fullnameInput.Text.ToString();
        user.login = usernameInput.Text.ToString();
        user.password = passwordInput.Text.ToString();
        user.createdAt = dateInput.Date + timeInput.Time;
        return user;
    }

    private void OnBack()
    {
        Application.RequestStop();
    }

    private bool ValidateInput()
    {
        if(usernameInput.Text.IsEmpty || passwordInput.Text.IsEmpty || fullnameInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all the fields", "OK").ToString();
            return false;
        }
        User user = this.GetUser();
        bool registered = Authentication.RegisterUser(user, userRepo);
        if(!registered)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Username \"{this.usernameInput.Text}\" already exist", "OK").ToString();
            return false;
        }
        return true;
    }

    private void OnQuit()
    {
        this.wantToQuit = true;
        Application.RequestStop();
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