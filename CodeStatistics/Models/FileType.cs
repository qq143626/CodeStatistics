using Prism.Mvvm;

namespace CodeStatistics.Models;

public class FileType : BindableBase
{
    private bool enabled;
    public bool Enabled
    {
        get => enabled;
        set => SetProperty(ref enabled, value);
    }

    private string? codeType;
    public string CodeType
    {
        get => codeType ?? string.Empty;
        set => SetProperty(ref codeType, value);
    }

    private string? extension;
    public string Extension
    {
        get => extension ?? string.Empty;
        set => SetProperty(ref extension, value);
    }

    private string? singleComments;
    public string SingleComments
    {
        get => singleComments ?? string.Empty;
        set => SetProperty(ref singleComments, value);
    }

    private string? multiCommentsStart;
    public string MultiCommentsStart
    {
        get => multiCommentsStart ?? string.Empty;
        set => SetProperty(ref multiCommentsStart, value);
    }

    private string? multiCommentsEnd;
    public string MultiCommentsEnd
    {
        get => multiCommentsEnd ?? string.Empty;
        set => SetProperty(ref multiCommentsEnd, value);
    }
}
