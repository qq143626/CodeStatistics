using Prism.Mvvm;

namespace CodeStatistics.Models;

public class CodeDirectory : BindableBase
{
    private bool enabled;
    public bool Enabled
    {
        get => enabled;
        set => SetProperty(ref enabled, value);
    }

    private string? directory;
    public string Directory
    {
        get => directory ?? string.Empty;
        set => SetProperty(ref directory, value);
    }
}
