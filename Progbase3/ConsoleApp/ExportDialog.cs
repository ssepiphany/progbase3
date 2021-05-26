using Terminal.Gui;

public class ExportDialog : Dialog
{
    private UserRepository userRepository;
    private string dialogTitle;
    public bool canceled;
    private ReviewRepository reviewRepository;
    public TextField dirPathInput;
    private TextField userIdInput;
    public User user;
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


        Label userIdLabl = new Label("User id:")
        {
            X = 6, Y = 4, Width = 10,
        };

        userIdInput = new TextField() 
        {
            X = rightColumn, Y = Pos.Top(userIdLabl) , Width = Dim.Fill() - 3, 
        };

        this.Add(userIdInput, userIdLabl);

        Label dirLabel = new Label("Dir path:")
        {
            X = 6, Y = 8, Width = 10,
        };

        dirPathInput = new TextField()
        {
            X = rightColumn, Y = Pos.Top(dirLabel), Width = Dim.Fill() - 3, Text = "not selected",
        };
        this.Add(dirPathInput);

        Button chooseDirBtn = new Button("choose directory")
        {
            X = 6, Y = Pos.Top(dirLabel) + 3, Width = 20,
        };
        chooseDirBtn.Clicked += OnChooseDirectory;
        this.Add(dirLabel, chooseDirBtn);
    }

    public void SetRepositories(UserRepository userRepository, ReviewRepository reviewRepository)
    {
        this.userRepository = userRepository;
        this.reviewRepository = reviewRepository;
    }


    private void OnChooseDirectory()
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

    private void OnSubmit()
    {
        if(!this.ValidateInput())
        {
            this.Title = this.dialogTitle;
            return;
        }
        this.user = reviewRepository.GetReviewsForExport(this.user);

        if(this.user.reviews.Count == 0)
        {
            this.Title = MessageBox.ErrorQuery("Error", $"Chosen user does not have any reviews", "OK").ToString();
            this.Title = this.dialogTitle;
            return;
        }

        this.canceled = false;
        Application.RequestStop();
    }

    private bool ValidateInput()
    {
        if(this.dirPathInput.Text == "not selected" || this.userIdInput.Text.IsEmpty)
        {
            this.Title = MessageBox.ErrorQuery("Error", "Please, make sure to fill all fields", "OK").ToString();
            return false;
        }
        int id;
        if(!int.TryParse(userIdInput.Text.ToString(), out id))
        {
            this.Title = MessageBox.ErrorQuery("Error", "Invalid user id", "OK").ToString();
            return false;
        }
        user = userRepository.GetById(id);
        if(user == null)
        {
            this.Title = MessageBox.ErrorQuery("Error", "User was not found", "OK").ToString();
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