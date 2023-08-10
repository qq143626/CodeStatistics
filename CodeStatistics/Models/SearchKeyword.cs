using Prism.Mvvm;

namespace CodeStatistics.Models;

public class SearchKeyword : BindableBase
{
    private bool enabled;
    public bool Enabled
    {
        get => enabled;
        set => SetProperty(ref enabled, value);
    }

    private string? keyword;
    public string Keyword
    {
        get => keyword ?? string.Empty;
        set => SetProperty(ref keyword, value);
    }
}
