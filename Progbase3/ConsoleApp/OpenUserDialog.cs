using Terminal.Gui;
using System;

public class OpenUserDialog : Dialog
{
    public bool deleted;
    public bool updated;
    public User user;
    private TextField idInput;
    private string dialogTitle;
    private TextField fullnameInput;
    private TextField loginInput;
    private TextField dateInput;
    private User currentUser;
    private Button editBtn;
    private Button deleteBtn;
    public OpenUserDialog()
    {
        this.dialogTitle = "Open user";
        this.Title = this.dialogTitle;

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogSubmit;

        this.AddButton(backBtn);

        int rightColumn = 20;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(idLbl, idInput);

        Label fullnameLbl = new Label(2, 4, "Fullname:");
        fullnameInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(fullnameLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(fullnameLbl, fullnameInput);

        Label loginLbl = new Label(2, 6, "Login:");
        loginInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(loginLbl), Width = 40, ReadOnly = true
        };
        this.Add(loginLbl, loginInput);


        Label dateTimeLbl = new Label(2, 8, "Date and time:");
        dateInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(dateTimeLbl),Width = 40, ReadOnly = true,
        };

        this.Add(dateTimeLbl, dateInput);

        editBtn = new Button(2, 22, "Edit");
        editBtn.Clicked += OnUserEdit;
        this.Add(editBtn);

        deleteBtn = new Button("Delete")
        {
            X = Pos.Right(editBtn) + 2, Y = Pos.Top(editBtn), 
        };
        deleteBtn.Clicked += OnUserDelete;
        this.Add(deleteBtn);

    }

    public void SetCurrentUser(User user)
    {
        this.currentUser = user;
        this.editBtn.Visible = this.user.id == this.currentUser.id;
        this.deleteBtn.Visible = this.currentUser.moderator || (this.user.id == this.currentUser.id);
    }

    private void OnUserDelete()
    {
        int index = MessageBox.Query("Delete user", "Are you sure", "No", "Yes");
        if(index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }

    private void OnUserEdit()
    {
        EditUserDialog dialog = new EditUserDialog(this.user);
        dialog.SetUser(this.user);
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            User updatedUser = dialog.GetUser();
            this.updated = true;
            updatedUser.id = user.id;
            this.SetUser(updatedUser);
        }
    }

    public void SetUser(User user)
    {
        this.user = user;
        this.idInput.Text = user.id.ToString();
        this.fullnameInput.Text = user.fullname;
        this.loginInput.Text = user.login;
        this.dateInput.Text = user.createdAt.ToString("F");
    }

    private void OnOpenDialogSubmit()
    {
        Application.RequestStop();
    }
}