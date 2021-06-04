using Terminal.Gui;

public class AddCastDialog : CreateActorDialog
{
    public int selectedItem;
    public AddCastDialog()
    {
        this.dialogTitle = "Add cast member";
        this.Title = this.dialogTitle;

        NStack.ustring[] options2 = new NStack.ustring[]{ "create actor", "choose actor"};
        RadioGroup choiceGroup = new RadioGroup
        {
            X = 2, Y = 20, RadioLabels = options2, 
        };
        choiceGroup.SelectedItemChanged += OnSelectedItemChanged;

        this.Add(choiceGroup);
    }

    public string GetActorsFullname()
    {
        return this.fullnameInput.Text.ToString();
    }

    protected void OnSelectedItemChanged(RadioGroup.SelectedItemChangedArgs args)
    {
        this.selectedItem = args.SelectedItem;
        if(this.selectedItem == 0)
        {
            this.Add(genderGroup);
            this.Add(actorGengerLbl);
            this.Add(ageLbl);
            this.Add(ageInput);
        }
        if(this.selectedItem == 1)
        {
            this.Remove(genderGroup);
            this.Remove(actorGengerLbl);
            this.Remove(ageLbl);
            this.Remove(ageInput);
        }
    }

    protected override bool ValidateInput()
    {
        if(this.fullnameInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        if(this.selectedItem == 0)
        {
            if(this.ageInput.Text.IsEmpty)
            {
                this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
                return false;
            }
            int age;
            if(!int.TryParse(this.ageInput.Text.ToString(), out age) )
            {
                this.Title = MessageBox.ErrorQuery("Error", "Invalid age value", "OK").ToString();
                return false;
            }
            if(actorRepository.GetByFullname(fullnameInput.Text.ToString()) != null)
            {
                this.Title = MessageBox.ErrorQuery("Error", $"Actor with fullname \"{this.fullnameInput.Text}\"\r\nalready exists", "OK").ToString();
                return false;
            }
            return true;
        }
        if(actorRepository.GetByFullname(fullnameInput.Text.ToString()) == null)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Actor \"{this.fullnameInput.Text}\" does not exist", "OK").ToString();
            return false;
        }
        return true;
    }
}