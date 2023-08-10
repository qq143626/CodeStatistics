namespace CodeStatistics.Models;

public class FileStatistic
{
    public string? FileName { get; init; }
    public string? FileType { get; init; }
    public string? FileDirectory { get; init; }
    public string? Keyword { get; init; }
    public long FileSize { get; init; }
    public int TotalLine { get; init; }
    public int CodeLine { get; init; }
    public int CommentLine { get; init; }
    public int BlankLine { get; init; }
}
