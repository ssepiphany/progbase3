using Terminal.Gui;

public class GenerateReportDialog : ExportDialog
{
    public GenerateReportDialog()
    {
        this.dialogTitle = "Report";
        this.Title = this.dialogTitle;
    }

    protected override void OnSubmit()
    {
        if(!this.ValidateInput())
        {
            this.Title = this.dialogTitle;
            return;
        }

        this.canceled = false;
        Application.RequestStop();
    }
}