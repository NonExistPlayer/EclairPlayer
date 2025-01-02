using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Eclair;

// Representing config.json as a .NET object.
internal sealed class ConfigJson
{
    #region Methods
    public static ConfigJson Load()
    {
        ConfigJson? config;
        if (!File.Exists(SavePath + "config.json"))
            return new ConfigJson().SaveAndReturn();

        try
        {
            config = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(SavePath + "config.json"));
            if (config == null)
                return new ConfigJson().SaveAndReturn();

            return config;
        }
        catch
        {
            return new ConfigJson().SaveAndReturn();
        }
    }

    public void Save()
    {
        File.WriteAllText(SavePath + "config.json", JsonConvert.SerializeObject(this));
        Logger.Log("config.json saved.");
    }
    private ConfigJson SaveAndReturn()
    {
        Save();
        return this;
    }

    public void LoadResources()
    {
        Logger.Log("Loading resources...");

        if (LoadResources(Theme))
            Logger.Log("Theme installed successfully.");
        else
        {
            Logger.Error($"Failed to set theme: '{Theme}'");
            Theme = "Default";
            Save();
            Logger.Log("Setting to default...", Notice);
            LoadResources("Default");
        }

        if (
            ((Avalonia.Platform.PlatformThemeVariant?)
             Application.Current?.ActualThemeVariant!) == Avalonia.Platform.PlatformThemeVariant.Light
            )
            LoadResources(Theme + "Light"); // can return false
    }
    public static bool LoadResources(string themename)
    {
        Uri uri = new($"avares://Eclair/Assets/{themename}Theme.axaml");

        if (Application.Current == null) return false;
        try
        {
            if (AvaloniaXamlLoader.Load(uri) is not ResourceDictionary resources) return false;
            ((App)Application.Current).SetResources(resources);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("App.SetResources() thren an exception:\n" + ex.ToString());
            return false;
        }
    }
    #endregion

    public string Theme = "Default";
    public bool UseCircleIconAnimation = true;
    public bool DisableCustomBorder = !OperatingSystem.IsWindows();
    public bool DisableEffects;
    public Color BackgroundColor = new(125, 0, 0, 0);
}