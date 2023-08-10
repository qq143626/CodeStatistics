using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeStatistics.Models;
using CodeStatistics.Models.Enums;

namespace CodeStatistics.Utils;

public class CodeStatistic
{
    private Action<AmountStatistics> amountStatisticsProgress;
    private Action<FileStatistic> fileStatisticsProgress;
    private Action finishCallback;
    private Action cancelCallback;

    private List<FileInfo> fileInfoList;
    private List<FileType>? fileTypeList;
    private List<string> keywords;
    private bool caseSensitive;
    private bool usingRegularity;
    private Stopwatch stopwatch;
    private CancellationTokenSource? tokenSource;

    private AmountStatistics? amountStatistics;
    private AmountStatistics AmountStatistics
    {
        get => amountStatistics ??= new();
        set => amountStatistics = value;
    }

    public CodeStatistic(
        Action<FileStatistic> fileStatisticsProgress,
        Action<AmountStatistics> amountStatisticsProgress,
        Action finishCallback,
        Action calcelCallback
    )
    {
        this.fileStatisticsProgress = fileStatisticsProgress;
        this.amountStatisticsProgress = amountStatisticsProgress;
        this.finishCallback = finishCallback;
        this.cancelCallback = calcelCallback;

        stopwatch = new Stopwatch();

        fileInfoList = new();
        keywords = new();
    }

    public void StatisticFilesSetting(
        List<string>? directories,
        List<FileType> fileTypeList,
        bool includeSubDir
    )
    {
        fileInfoList.Clear();

        if (directories == null || directories.Count <= 0)
        {
            finishCallback();
            return;
        }

        var infoList = new List<FileInfo>();
        this.fileTypeList = fileTypeList;
        string[] patterns;
        if (fileTypeList.Count > 0)
        {
            patterns = string.Join(';', fileTypeList.Select(x => x.Extension.Replace("*.", ".")))
                .Split(';');
        }
        else
        {
            patterns = new string[] { ".txt" };
        }
        foreach (string directory in directories)
        {
            var files = GetFiles(directory, patterns, includeSubDir);
            if (files != null)
            {
                infoList.AddRange(files);
            }
        }

        if (infoList.Count == 0)
        {
            finishCallback();
            return;
        }

        fileInfoList.AddRange(infoList.DistinctBy(x => x.FullName));

        AmountStatistics.StatisticDate = DateTime.Now.ToString("yyyy-MM-dd");
        AmountStatistics.FileDirectory = fileInfoList[0].DirectoryName;
        if (fileTypeList.Count > 0)
        {
            AmountStatistics.FileType = fileTypeList[0].CodeType;
        }

        amountStatisticsProgress(AmountStatistics);
    }

    public void StatisticKeywordsSetting(
        List<string>? keywords,
        bool caseSensitive,
        bool usingRegularity
    )
    {
        this.keywords.Clear();

        if (keywords == null || keywords.Count <= 0)
        {
            return;
        }

        this.keywords.AddRange(keywords);
        this.caseSensitive = caseSensitive;
        this.usingRegularity = usingRegularity;
    }

    public void StartStatistic()
    {
        if (fileInfoList.Count == 0)
        {
            finishCallback();
            return;
        }

        tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        token.Register(cancelCallback);
        Task.Run(
            async () =>
            {
                stopwatch.Restart();
                foreach (var fileInfo in fileInfoList)
                {
                    var fileStatistic = await GetFileStatistic(fileInfo);
                    if (fileStatistic != null)
                    {
                        AmountStatistics.FileCount = AmountStatistics.FileCount + 1;
                        AmountStatistics.FileSize =
                            AmountStatistics.FileSize + fileStatistic.FileSize;
                        AmountStatistics.TotalLine =
                            AmountStatistics.TotalLine + fileStatistic.TotalLine;
                        AmountStatistics.CodeLine =
                            AmountStatistics.CodeLine + fileStatistic.CodeLine;
                        AmountStatistics.CommentLine =
                            AmountStatistics.CommentLine + fileStatistic.CommentLine;
                        AmountStatistics.BlankLine =
                            AmountStatistics.BlankLine + fileStatistic.BlankLine;

                        fileStatisticsProgress(fileStatistic);
                    }

                    AmountStatistics.TimeConsuming = stopwatch.ElapsedMilliseconds;
                    amountStatisticsProgress(AmountStatistics);
                }
                stopwatch.Stop();
                AmountStatistics.TimeConsuming = stopwatch.ElapsedMilliseconds;
                amountStatistics = null;

                finishCallback();
            },
            token
        );
    }

    public void StopStatistic()
    {
        tokenSource?.Cancel();
    }

    private async Task<FileStatistic?> GetFileStatistic(FileInfo fileInfo)
    {
        var keyword = string.Empty;
        var totalLine = 0;
        var codeLine = 0;
        var commentLine = 0;
        var blankLine = 0;
        var codeLineState = CodeLineState.CodeLine;
        using StreamReader stream = new(fileInfo.FullName);
        if (keywords.Count > 0)
        {
            stream.BaseStream.Seek(0, SeekOrigin.Begin);
            var text = await stream.ReadToEndAsync();
            if (!MatchKeyword(text, ref keyword))
            {
                return null;
            }
        }
        stream.BaseStream.Seek(0, SeekOrigin.Begin);
        var fileType = GetFileType(fileInfo);
        while (!stream.EndOfStream)
        {
            var line = await stream.ReadLineAsync() ?? string.Empty;
            totalLine++;
            GetCodeLineState(line, fileType, codeLineState);
            switch (codeLineState)
            {
                case CodeLineState.CodeLine:
                    codeLine++;
                    break;
                case CodeLineState.CommentLine:
                case CodeLineState.CommentLineStart:
                case CodeLineState.CommentLineEnd:
                    commentLine++;
                    break;
                case CodeLineState.BlankLine:
                    blankLine++;
                    break;
            }
        }

        return new FileStatistic()
        {
            FileName = fileInfo.Name,
            FileType = fileType?.CodeType ?? fileInfo.Extension,
            FileDirectory = Path.GetDirectoryName(fileInfo.FullName),
            Keyword = keyword,
            FileSize = fileInfo.Length,
            TotalLine = totalLine,
            CodeLine = codeLine,
            CommentLine = commentLine,
            BlankLine = blankLine,
        };
    }

    private CodeLineState GetCodeLineState(string line, FileType? fileType, CodeLineState preState)
    {
        if (fileType == null)
        {
            return string.IsNullOrEmpty(line) ? CodeLineState.BlankLine : CodeLineState.CodeLine;
        }

        if (preState == CodeLineState.CommentLineStart)
        {
            if (line.StartsWith(fileType.MultiCommentsEnd))
            {
                return CodeLineState.CommentLineEnd;
            }
            else
            {
                return CodeLineState.CommentLineStart;
            }
        }
        if (string.IsNullOrEmpty(line))
        {
            return CodeLineState.BlankLine;
        }
        if (line.StartsWith(fileType.SingleComments))
        {
            return CodeLineState.CommentLine;
        }
        return CheckMultiCommentStart(line, fileType.MultiCommentsStart, fileType.MultiCommentsEnd);
    }

    private CodeLineState CheckMultiCommentStart(
        string line,
        string multiCommentStart,
        string multiCommentEnd
    )
    {
        if (line.StartsWith(multiCommentStart))
        {
            if (line.EndsWith(multiCommentEnd))
            {
                return CodeLineState.CommentLine;
            }
            else
            {
                return CodeLineState.CommentLineStart;
            }
        }
        return CodeLineState.CodeLine;
    }

    private FileType? GetFileType(FileInfo fileInfo)
    {
        if (fileTypeList == null)
        {
            return null;
        }

        foreach (var fileType in fileTypeList)
        {
            foreach (var extension in fileType.Extension.Split(';'))
            {
                if (extension.TrimStart('*') == fileInfo.Extension)
                {
                    return fileType;
                }
            }
        }
        return null;
    }

    private bool MatchKeyword(string text, ref string matchKeyword)
    {
        foreach (var keyword in keywords)
        {
            if (usingRegularity)
            {
                var regexOption = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                if (Regex.IsMatch(text, keyword, regexOption))
                {
                    matchKeyword = keyword;
                    return true;
                }
            }
            else
            {
                var comparison = caseSensitive
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;
                if (text.Contains(keyword, comparison))
                {
                    matchKeyword = keyword;
                    return true;
                }
            }
        }
        matchKeyword = string.Empty;
        return false;
    }

    private List<FileInfo>? GetFiles(
        string directory,
        IEnumerable<string> patterns,
        bool includeSubDir
    )
    {
        if (File.Exists(directory))
        {
            return new List<FileInfo> { new FileInfo(directory) };
        }
        else if (Directory.Exists(directory))
        {
            DirectoryInfo directoryInfo = new(directory);
            var result = new List<FileInfo>();
            FileInfo[] fileInfos;
            if (includeSubDir)
            {
                fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            }
            else
            {
                fileInfos = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            }
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (MatchExtension(fileInfo, patterns))
                {
                    result.Add(fileInfo);
                }
            }
            return result;
        }
        else
        {
            return null;
        }
    }

    private bool MatchExtension(FileInfo fileInfo, IEnumerable<string> patterns)
    {
        if (string.IsNullOrEmpty(fileInfo.Extension))
        {
            return patterns.Contains(fileInfo.Name);
        }
        else
        {
            return patterns.Contains(fileInfo.Extension);
        }
    }
}
