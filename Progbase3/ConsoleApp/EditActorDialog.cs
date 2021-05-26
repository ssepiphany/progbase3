using Terminal.Gui;

public class EditActorDialog : CreateActorDialog
{
    
    private TextField idInput;
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
        this.idInput.Text = actor.id.ToString();
        this.fullnameInput.Text = actor.fullname;
        this.genderGroup.SelectedItem = GetIndexOfSelectedItem(actor.gender);
        this.ageInput.Text = actor.age.ToString();
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