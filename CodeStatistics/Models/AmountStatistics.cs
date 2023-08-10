using System;
using System.Text;
using Prism.Mvvm;

namespace CodeStatistics.Models;

public class AmountStatistics : BindableBase
{
    private string? statisticDate;
    public string? StatisticDate
    {
        get => statisticDate;
        set => SetProperty(ref statisticDate, value);
    }

    private string? fileDirectory;
    public string? FileDirectory
    {
        get => fileDirectory;
        set => SetProperty(ref fileDirectory, value);
    }

    private string? fileType;
    public string? FileType
    {
        get => fileType;
        set => SetProperty(ref fileType, value);
    }

    private long timeConsuming;
    public long TimeConsuming
    {
        get => timeConsuming;
        set
        {
            if (SetProperty(ref timeConsuming, value))
            {
                RaisePropertyChanged(nameof(TimeConsumingStr));
            }
        }
    }

    public string TimeConsumingStr
    {
        get
        {
            var ticks = timeConsuming;
            TimeSpan timeSpan = new TimeSpan(ticks);
            StringBuilder builder = new();
            if (timeSpan.Hours > 0)
            {
                builder.Append($"{timeSpan.Hours}小时");
            }
            if (timeSpan.Minutes > 0)
            {
                builder.Append($"{timeSpan.Minutes}分");
            }
            builder.Append($"{timeSpan.Seconds}");
            builder.Append($".{timeSpan.Milliseconds.ToString().PadLeft(3, '0')}秒");

            return builder.ToString();
        }
    }

    private int fileCount;
    public int FileCount
    {
        get => fileCount;
        set => SetProperty(ref fileCount, value);
    }

    private long fileSize;
    public long FileSize
    {
        get => fileSize;
        set
        {
            if (SetProperty(ref fileSize, value))
            {
                RaisePropertyChanged(nameof(FileSizeStr));
            }
        }
    }

    public string FileSizeStr
    {
        get
        {
            var fileSize = (double)FileSize;
            if (fileSize > 1024 * 1024 * 1024)
            {
                return string.Format("{0:N2}GB", fileSize / 1024 / 1024 / 1024);
            }
            else if (fileSize > 1024 * 1024)
            {
                return string.Format("{0:N2}MB", fileSize / 1024 / 1024);
            }
            else if (fileSize > 1024)
            {
                return string.Format("{0:N2}KB", fileSize / 1024);
            }
            else
            {
                return string.Format("{0}B", fileSize);
            }
        }
    }

    private int totalLine;
    public int TotalLine
    {
        get => totalLine;
        set => SetProperty(ref totalLine, value);
    }

    private int codeLine;
    public int CodeLine
    {
        get => codeLine;
        set => SetProperty(ref codeLine, value);
    }

    private int commentLine;
    public int CommentLine
    {
        get => commentLine;
        set => SetProperty(ref commentLine, value);
    }

    private int blankLine;
    public int BlankLine
    {
        get => blankLine;
        set => SetProperty(ref blankLine, value);
    }

    public void AssignTo(AmountStatistics? amountStatistics)
    {
        if (amountStatistics == null)
        {
            return;
        }
        amountStatistics.StatisticDate = StatisticDate;
        amountStatistics.FileDirectory = FileDirectory;
        amountStatistics.FileType = FileType;
        amountStatistics.TimeConsuming = TimeConsuming;
        amountStatistics.FileCount = FileCount;
        amountStatistics.FileSize = FileSize;
        amountStatistics.TotalLine = TotalLine;
        amountStatistics.CodeLine = CodeLine;
        amountStatistics.CommentLine = CommentLine;
        amountStatistics.BlankLine = BlankLine;
    }
}
