using System;

namespace Eclair.ViewModels;

public class ViewModel : ViewModelBase
{
    public string Settings => resources.ui_settings;
    public string SelectFile => resources.ui_selectfile;
    public string Back => resources.ui_player_back;
    public string About => resources.ui_about;
    public string Search
    { 
        get
        {
            var today = DateTime.Now;

            if (today >= NewYear_Start && today <= NewYear_End)
                return "🎉" + resources.ui_search_newyear + "🎉";
            else if (today >= EclairBirthday_Start && today <= EclairBirthday_End)
                return "🎂" + resources.ui_search_birthday + "🎂";
            else if (today.Day == 23 && today.Month == 11) // 23 November 2012
                return resources.ui_search_nonex + " <3";

            return resources.ui_search;
        }
    }
    public string NothingFound => resources.ui_nothingfound + " :(";

    public string About_Version => resources.ui_about_version + Main.Version;
    public string About_CreatedBy => "Eclair " + resources.ui_about_createdby + " NonExistPlayer";
    public string About_OriginalRepo => resources.ui_about_originalrepo;
    public string About_License => resources.ui_about_license;
}