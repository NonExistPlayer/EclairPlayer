namespace Eclair.ViewModels;

public class ViewModel : ViewModelBase
{
    public string Settings => resources.ui_settings;
    public string SelectFile => resources.ui_selectfile;
    public string About => resources.ui_about;

    public string About_CreatedBy => resources.ui_about_createdby + " NonExistPlayer";
    public string About_OriginalRepo => resources.ui_about_originalrepo;
}