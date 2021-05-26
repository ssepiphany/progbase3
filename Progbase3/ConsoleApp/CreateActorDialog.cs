using Terminal.Gui;

public class CreateActorDialog : Dialog
{
    public bool canceled; 
    protected string dialogTitle;
    protected TextField fullnameInput;
    protected RadioGroup genderGroup;
    protected NStack.ustring[] options;
    protected TextField ageInput;
    public CreateActorDialog()
    {
        this.dialogTitle = "Create actor";
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

        Label actorGengerLbl = new Label(2, 6, "Gender:");
        options = new NStack.ustring[]{"male" , "female"};
        genderGroup = new RadioGroup(rightColumn, 6, options);

        this.Add(actorGengerLbl, genderGroup);


        Label ageLbl = new Label(2, 7 + options.Length , "Age:");
        ageInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(ageLbl), Width = 40,
        };
        this.Add(ageLbl, ageInput);

    }

    public Actor GetActor()
    {
        Actor actor = new Actor();
        actor.fullname = this.fullnameInput.Text.ToString();
        actor.age = int.Parse(this.ageInput.Text.ToString());
        actor.gender = options[genderGroup.SelectedItem].ToString(); 
        return actor;
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
        if(this.fullnameInput.Text.IsEmpty || this.ageInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        int age;
        if(!int.TryParse(this.ageInput.Text.ToString(), out age))
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid age value", "OK").ToString();
            return false;
        }
        return true;
    }
}