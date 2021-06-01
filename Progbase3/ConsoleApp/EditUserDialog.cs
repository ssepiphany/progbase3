using Terminal.Gui;

public class EditUserDialog : CreateUserDialog
{
    
    private TextField idInput;
    private TextField dateText;
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
        this.idInput.Text = user.id.ToString();
        this.fullnameInput.Text = user.fullname;
        this.loginInput.Text = user.login;
        this.dateText.Text = user.createdAt.ToString("F");
        this.moderatorCheck.Checked = user.moderator;
    }
}