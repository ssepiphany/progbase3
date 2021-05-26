using System;
using Terminal.Gui;


public class CreateUserDialog : Dialog
{
    public bool canceled; 
    protected string dialogTitle;
    protected TextField fullnameInput;
    protected TextField loginInput;
    protected DateField dateInput;
    protected Label dateTimeLbl;
    protected TimeField timeInput;
    public CreateUserDialog()
    {
        this.dialogTitle = "Create user";
        this.Title = this.dialogTitle;
        Button okBtn = new Button("OK");
        okBtn.Clicked += OnCreateDialogSubmit;

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCreateDialogCanceled;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        int rightColumn = 20;

        Label fullnameLbl = new Label(2, 4, "Fullname:");
        fullnameInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(fullnameLbl), Width = 40
        };
        this.Add(fullnameLbl, fullnameInput);

        Label loginLbl = new Label(2, 6, "Login:");
        loginInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(loginLbl), Width = 40,
        };
        this.Add(loginLbl, loginInput);


        dateTimeLbl = new Label(2, 8, "Date and time:");
        dateInput = new DateField()
        {
            X = rightColumn, Y = Pos.Top(dateTimeLbl), Date = DateTime.Now, Width = 12, ReadOnly = true, 
        };

        timeInput = new TimeField()
        {
            X = rightColumn + 12, Y = Pos.Top(dateTimeLbl), Time = DateTime.Now.TimeOfDay, Width = 28, ReadOnly = true,
        };

        this.Add(dateTimeLbl, dateInput, timeInput);

    }

    public User GetUser()
    {
        User user = new User();
        user.fullname = this.fullnameInput.Text.ToString(); 
        user.login = this.loginInput.Text.ToString();
        user.createdAt = this.dateInput.Date + this.timeInput.Time;
        return user;
    }

    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }

    private void OnCreateDialogSubmit()
    {
        if(!ValidateInput()) 
        {
            this.Title = this.dialogTitle; 
            return;
        }
        this.canceled = false;
        Application.RequestStop();
    }

    public bool ValidateInput()
    {
        if(this.fullnameInput.Text.IsEmpty || this.loginInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        return true;
    }
}