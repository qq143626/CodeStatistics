using System;
using System.Collections.ObjectModel;
using CodeStatistics.Models;
using CodeStatistics.Models.Enums;
using CodeStatistics.Models.Events;
using CodeStatistics.Utils;
using Prism.Events;

namespace CodeStatistics.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    public const string Tag = "HistoryRegion";

    public ObservableCollection<HistoryStatistics> HistoryStatisticsDatas { get; init; }

    private HistoryStatistics? historyStatistics;
    private SaveStatisticState saveStatisticState;
    private object locker;

    public HistoryViewModel(IEventAggregator eventAggregator)
        : base(eventAggregator)
    {
        locker = new();
        HistoryStatisticsDatas = new();
        foreach (var historyFile in StorageManager.GetAllHistoryStatisticsFile())
        {
            var historyStatistics = JsonHelper.Deserialize<HistoryStatistics>(historyFile);
            if (historyStatistics != null)
            {
                HistoryStatisticsDatas.Add(historyStatistics);
            }
        }

        eventAggregator.GetEvent<SaveStatisticEvent>().Subscribe(InitHistoryStatistic);

        eventAggregator
            .GetEvent<SettingSetSaveEvent>()
            .Subscribe(
                (settingSet) =>
                {
                    historyStatistics?.CodeDirDatas?.AddRange(settingSet.Item1);
                    historyStatistics?.FileTypeDatas?.AddRange(settingSet.Item2);
                    historyStatistics?.SearchKeywordData?.AddRange(settingSet.Item3);
                    CheckSaveStatisticState(SaveStatisticState.SettingSet);
                }
            );

        eventAggregator
            .GetEvent<FileStatisticSaveEvent>()
            .Subscribe(
                (fileStatistic) =>
                {
                    historyStatistics?.FileStatisticDatas?.AddRange(fileStatistic);
                    CheckSaveStatisticState(SaveStatisticState.FileStatisticSet);
                }
            );

        eventAggregator
            .GetEvent<AmountStatisticsSaveEvent>()
            .Subscribe(
                (amountStatistics) =>
                {
                    amountStatistics.AssignTo(historyStatistics?.AmountStatistics);
                    CheckSaveStatisticState(SaveStatisticState.AmountStatisticSet);
                }
            );
    }

    private void CheckSaveStatisticState(SaveStatisticState currStatisticState)
    {
        lock (locker)
        {
            saveStatisticState |= currStatisticState;

            if (saveStatisticState == SaveStatisticState.AllSet)
            {
                if (historyStatistics != null && historyStatistics?.FileStatisticDatas?.Count > 0)
                {
                    JsonHelper.Serialize(historyStatistics, StorageManager.GetHistoryStatistics());
                    HistoryStatisticsDatas.Add(historyStatistics);
                }
            }
        }
    }

    private void InitHistoryStatistic()
    {
        historyStatistics = new()
        {
            CodeDirDatas = new(),
            FileTypeDatas = new(),
            SearchKeywordData = new(),
            FileStatisticDatas = new(),
            AmountStatistics = new(),
        };
        saveStatisticState = SaveStatisticState.Unset;
        eventAggregator.GetEvent<SaveStatisticReadyEvent>().Publish();
    }
}
