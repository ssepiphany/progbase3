using Terminal.Gui;

public class OpenActorDialog : Dialog
{
    public bool deleted;
    public bool updated;
    private Actor actor;
    private TextField idInput; 
    private TextField fullnameInput;
    private TextField genderValue;
    private TextField ageInput;
    public OpenActorDialog()
    {
        this.Title = "Open actor";

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnOpenDialogSubmit;

        this.AddButton(backBtn);

        int rightColumn = 20;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true
        };
        this.Add(idLbl, idInput);


       Label fullnameLbl = new Label(2, 4, "Fullname:");
        fullnameInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(fullnameLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(fullnameLbl, fullnameInput);

        Label actorGengerLbl = new Label(2, 6, "Gender:");

        genderValue = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(actorGengerLbl), Width = 40, ReadOnly = true,
        };

        this.Add(actorGengerLbl, genderValue);


        Label ageLbl = new Label(2, 8, "Age:");
        ageInput = new TextField("")
        {
            X = rightColumn, Y = Pos.Top(ageLbl), Width = 40, ReadOnly = true, 
        };
        this.Add(ageLbl, ageInput);


        Button editBtn = new Button(2, 22, "Edit");
        editBtn.Clicked += OnActorEdit;
        this.Add(editBtn);

        Button deleteBtn = new Button("Delete")
        {
            X = Pos.Right(editBtn) + 2, Y = Pos.Top(editBtn), 
        };
        deleteBtn.Clicked += OnActorDelete;
        this.Add(deleteBtn);
    }

    private void OnActorDelete()
    {
        int index = MessageBox.Query("Delete actor", "Are you sure", "No", "Yes");
        if(index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }

    private void OnActorEdit()
    {
        EditActorDialog dialog = new EditActorDialog();
        dialog.SetActor(this.actor);
        Application.Run(dialog);

        if(!dialog.canceled)
        {
            Actor updatedActor = dialog.GetActor();
            this.updated = true;
            updatedActor.id = actor.id;
            this.SetActor(updatedActor);
        }
    }

    public Actor GetActor()
    {
        return this.actor;
    }

    public void SetActor(Actor actor)
    {
        this.actor = actor;
        this.idInput.Text = actor.id.ToString();
        this.ageInput.Text = actor.age.ToString();
        this.fullnameInput.Text = actor.fullname;
        this.genderValue.Text = actor.gender;
    }

    private void OnOpenDialogSubmit()
    {
        Application.RequestStop();
    }
}