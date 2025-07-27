using System;
using Avalonia.Markup.Xaml;

namespace Eclair;

public class LocalizeExtension(string key) : MarkupExtension
{
    public string Key { get; } = key;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        Localization.ResourceManager.GetString(Key) ?? Key;
}