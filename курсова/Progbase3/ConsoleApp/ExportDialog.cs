using Terminal.Gui;

public class ExportDialog : Dialog
{
    protected string dialogTitle;
    public bool canceled;
    protected ReviewRepository reviewRepository;
    protected TextField dirPathInput;
    protected User currentUser;
    public ExportDialog()
    {
        this.dialogTitle = "Export";
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

        Label dirLabel = new Label("Dir path:")
        {
            X = 6, Y = 8, Width = 10,
        };

        dirPathInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(dirLabel), Width = Dim.Fill() - 3, Text = "not selected",
        };
        this.Add(dirPathInput);

        Button chooseDirBtn = new Button("select directory")
        {
            X = 6, Y = Pos.Top(dirLabel) + 3, Width = 20,
        };
        chooseDirBtn.Clicked += OnChooseDirectory;
        this.Add(dirLabel, chooseDirBtn);
    }

    public void SetRepositories(ReviewRepository reviewRepository)
    {
        this.reviewRepository = reviewRepository;
    }

    public void SetCurrentUser(ref User user)
    {
        this.currentUser = user;
    }

    public string GetDirPath()
    {
        return this.dirPathInput.Text.ToString();
    }

    protected void OnChooseDirectory()
    {
        OpenDialog chooseFileDialog = new OpenDialog("Choose directory", "");
        chooseFileDialog.CanChooseDirectories = true;
        chooseFileDialog.CanChooseFiles = false;
        chooseFileDialog.CanCreateDirectories = true;

        Application.Run(chooseFileDialog);
         
        if(!chooseFileDialog.Canceled)
        {
            NStack.ustring filePath = chooseFileDialog.FilePath;
            dirPathInput.Text = filePath;
        }

    }

    protected virtual void OnSubmit()
    {
        if(!this.ValidateInput())
        {
            this.Title = this.dialogTitle;
            return;
        }
        this.currentUser = reviewRepository.GetReviewsForExport(this.currentUser);

        if(this.currentUser.reviews.Count == 0)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"You do not have any reviews", "OK").ToString();
            this.Title = this.dialogTitle;
            return;
        }

        this.canceled = false;
        Application.RequestStop();
    }

    protected bool ValidateInput()
    {
        if(this.dirPathInput.Text == "not selected")
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to select directory", "OK").ToString();
            return false;
        }
        
        return true;
    }

    protected void OnExit()
    {
        this.canceled = true;
        Application.RequestStop();
    }
}