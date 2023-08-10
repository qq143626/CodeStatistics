using System.Collections.Generic;

namespace CodeStatistics.Models;

public class HistoryStatistics
{
    public List<CodeDirectory>? CodeDirDatas { get; init; }
    public List<FileType>? FileTypeDatas { get; init; }
    public List<SearchKeyword>? SearchKeywordData { get; init; }
    public List<FileStatistic>? FileStatisticDatas { get; init; }
    public AmountStatistics? AmountStatistics { get; init; }
}
