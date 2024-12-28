namespace Eclair.ViewModels;

public class ViewModel : ViewModelBase
{
    public string Settings => resources.ui_settings;
    public string SelectFile => resources.ui_selectfile;
    public string Back => resources.ui_player_back;
    public string About => resources.ui_about;

    public string About_Version => resources.ui_about_version + Version;
    public string About_CreatedBy => "Eclair " + resources.ui_about_createdby + " NonExistPlayer";
    public string About_OriginalRepo => resources.ui_about_originalrepo;
    public string About_License => resources.ui_about_license;
}