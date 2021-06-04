using Terminal.Gui;

public class EditActorDialog : CreateActorDialog
{
    
    private TextField idInput;
    protected Actor actor; 
    public EditActorDialog()
    {
        this.dialogTitle = "Edit actor";
        this.Title = this.dialogTitle;

        Label idLbl = new Label(2, 2, "Id");
        idInput = new TextField("")
        {
            X = 20, Y = Pos.Top(idLbl), Width = 40, ReadOnly = true
        };
        this.Add(idLbl, idInput);
    }

    public void SetActor(Actor actor)
    {
        this.actor = actor;
        this.idInput.Text = actor.id.ToString();
        this.fullnameInput.Text = actor.fullname;
        this.genderGroup.SelectedItem = GetIndexOfSelectedItem(actor.gender);
        this.ageInput.Text = actor.age.ToString();
    }

    protected override bool ValidateInput()
    {
        if(this.fullnameInput.Text.IsEmpty || this.ageInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        if(actorRepository.GetByFullname(fullnameInput.Text.ToString()) != null && this.fullnameInput.Text.ToString() != this.actor.fullname)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Actor with fullname \"{this.fullnameInput.Text}\"\r\nalready exists", "OK").ToString();
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

    private int GetIndexOfSelectedItem(string item)
    {
        for(int i = 0; i < options.Length; i++)
        {
            if(options[i] == item)
            {
                return i;
            }
        }
        return -1;
    }
}