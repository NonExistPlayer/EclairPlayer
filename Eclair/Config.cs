using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Eclair;

// Representing config.json as a .NET object.
internal sealed class Config
{
    public static Config Load()
    {
        Config? config;
        if (!File.Exists(App.SavePath + "config.json"))
            return new Config().SaveAndReturn();

        try
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(App.SavePath + "config.json"));
            if (config == null)
                return new Config().SaveAndReturn();

            return config;
        }
        catch
        {
            return new Config().SaveAndReturn();
        }
    }

    public void Save()
    {
        File.WriteAllText(App.SavePath + "config.json", JsonConvert.SerializeObject(this));
        Logger.WriteLine("config.json saved.");
    }
    private Config SaveAndReturn()
    {
        Save();
        return this;
    }

    public string Theme = "Default";
    public bool UseCircleIconAnimation = true;

    public void LoadResources()
    {
        Logger.WriteLine("Loading resources...");

        if (LoadResources(Theme))
            Logger.WriteLine("Theme installed successfully.");
        else
        {
            Logger.WriteLine($"Failed to set theme: '{Theme}'", Error);
            Theme = "Default";
            Save();
            Logger.WriteLine("Setting to default...", Notice);
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
            Logger.WriteLine("App.SetResources() thren an exception:\n" + ex.ToString(), Error);
            return false;
        }
    }
}