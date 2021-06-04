using Terminal.Gui;

public class EditUserDialog : CreateUserDialog
{
    
    private TextField idInput;
    private TextField dateText;
    protected User currentUser;
    public EditUserDialog(User user)
    {
        this.dialogTitle = "Edit user";
        this.Title = this.dialogTitle;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = 20, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true
        };
        this.Add(idLbl, idInput);

        dateInput.Visible = false;
        timeInput.Visible = false;
        dateText = new TextField()
        {
            X = 20, Y = Pos.Top(dateTimeLbl), Width = 40, ReadOnly = true,
        };
        this.Add(dateText);
    }

    public void SetUser(User user)
    {
        this.currentUser = user;
        this.idInput.Text = user.id.ToString();
        this.fullnameInput.Text = user.fullname;
        this.loginInput.Text = user.login;
        this.dateText.Text = user.createdAt.ToString("F");
        this.moderatorCheck.Visible = user.moderator; 
        this.moderatorCheck.Checked = user.moderator;
    }

    protected override bool ValidateInput()
    {
        if(this.fullnameInput.Text.IsEmpty || this.loginInput.Text.IsEmpty || this.passwordInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        if(this.repo.GetByLogin(this.loginInput.Text.ToString()) != null && this.loginInput.Text.ToString() != this.currentUser.login)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Username \"{this.loginInput.Text.ToString()}\" already exists", "OK").ToString();
            return false;
        }
        return true;
    }
}