// based on https://github.com/AvaloniaUI/Avalonia.Samples/tree/main/src/Avalonia.Samples/Routing/BasicViewLocatorSample

using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Eclair.Views;
using System;

namespace Eclair;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null)
        {
            Logger.WriteLine("data was null");
            return new TextBlock
            {
                Text = "data was null"
            };
        }

        var type = data.GetType();
        var name = type.FullName!;
        
        if (type != null)
            return (Control)Activator.CreateInstance(type)!;
        else
            return new TextBlock
            {
                Text = "Not Found: " + name,
            };
    }

    public bool Match(object? data) => data is MainView or SettingsView;
}