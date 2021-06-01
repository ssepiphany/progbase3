using Terminal.Gui;

public class ImportDialog : Dialog
{
    private string dialogTitle;
    public bool canceled;
    private TextField filePathInput;
    public ImportDialog()
    {
        this.dialogTitle = "Import";
        int rightColumn = 20;
        this.Title = this.dialogTitle;

        Button backBtn = new Button()
        {
            X = Pos.Center() - 3, Y = 0, Text = "Back",
        };

        Button okBtn = new Button()
        {
            X = Pos.Center() + 1, Y = 0, Text = "OK",
        };

        backBtn.Clicked += OnExit;
        okBtn.Clicked += OnSubmit;
        this.AddButton(backBtn);
        this.AddButton(okBtn);


        Label fileLabel = new Label("File path:")
        {
            X = 6, Y = 8, Width = 10,
        };

        filePathInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(fileLabel), Width = Dim.Fill() - 3, Text = "not selected", ReadOnly = true, 
        };
        this.Add(filePathInput);

        Button chooseFileBtn = new Button("select file")
        {
            X = 6, Y = Pos.Top(fileLabel) + 3, Width = 15,
        };
        chooseFileBtn.Clicked += OnChooseFile;
        this.Add(fileLabel, chooseFileBtn);
    }

    public string GetFilePath()
    {
       return this.filePathInput.Text.ToString();
    }

    private void OnChooseFile()
    {
        OpenDialog chooseFileDialog = new OpenDialog("Select file", "");
        chooseFileDialog.CanChooseDirectories = false;
        chooseFileDialog.CanChooseFiles = true;

        Application.Run(chooseFileDialog);
         
        if(!chooseFileDialog.Canceled)
        {
            NStack.ustring filePath = chooseFileDialog.FilePath;
            this.filePathInput.Text = filePath;
        }
    }

    private void OnSubmit()
    {
        if(!this.ValidateInput())
        {
            this.Title = this.dialogTitle;
            return;
        }

        this.canceled = false;
        Application.RequestStop();
    }

    private bool ValidateInput()
    {
        if(this.filePathInput.Text == "not selected")
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to select file", "OK").ToString();
            return false;
        }
        return true;
    }

    private void OnExit()
    {
        this.canceled = true;
        Application.RequestStop();
    }
}