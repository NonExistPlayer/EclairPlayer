using System;

namespace Eclair.ViewModels;

public class ViewModel : ViewModelBase
{
    public string Settings => resources.ui_settings;
    public string SelectFile => resources.ui_selectfile;
    public string SelectDir => resources.ui_selectdir;
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

    #region About
    public string About_Version => resources.ui_about_version + Main.Version;
    public string About_CreatedBy => "Eclair " + resources.ui_about_createdby + " NonExistPlayer";
    public string About_OriginalRepo => resources.ui_about_originalrepo;
    public string About_License => resources.ui_about_license;
    public string About_IconsSource => resources.ui_about_iconssource;
    public string About_Site => resources.ui_about_site;
    #endregion

    #region Icons
    public string Icons_About => resources.icons_about;
    public string Icons_Back => resources.icons_back;
    public string Icons_Forward => resources.icons_forward;
    public string Icons_Loop => resources.icons_loop;
    public string Icons_Pause => resources.icons_pause;
    public string Icons_Play => resources.icons_play;
    public string Icons_SelectFile => resources.icons_selectfile;
    public string Icons_SelectDir => resources.icons_selectdir;
    public string Icons_Settings => resources.icons_settings;
    public string Icons_Stop => resources.icons_stop;
    #endregion
}