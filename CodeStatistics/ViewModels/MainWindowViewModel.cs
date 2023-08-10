using System.Collections.ObjectModel;
using System.Windows;
using CodeStatistics.Models;
using CodeStatistics.Models.Events;
using CodeStatistics.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace CodeStatistics.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private IRegionManager regionManager;
    private IEventAggregator eventAggregator;

    private string message;

    public string Message
    {
        get => message;
        set => SetProperty(ref message, value);
    }

    private Visibility messageVisibility;

    public Visibility MessageVisibility
    {
        get => messageVisibility;
        set => SetProperty(ref messageVisibility, value);
    }

    private bool toggleIsChecked;
    public bool ToggleIsChecked
    {
        get => toggleIsChecked;
        set => SetProperty(ref toggleIsChecked, value);
    }

    private CodeStatistic codeStatistic;

    public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
    {
        this.regionManager = regionManager;
        this.eventAggregator = eventAggregator;

        message = string.Empty;
        messageVisibility = Visibility.Hidden;
        ToggleIsChecked = false;

        codeStatistic = new(
            FileStatisticsProgress,
            AmountStatisticsProgress,
            FinishCallback,
            CancelCallback
        );

        LoadedCommand = new DelegateCommand(ExecuteLoaded);
        ChangeWorkStateCommand = new(ExecuteChangeWorkState);
        SaveStatisticCommand = new(ExecuteSaveStatistic);
    }

    public DelegateCommand LoadedCommand { get; init; }

    private void ExecuteLoaded()
    {
        regionManager.RequestNavigate(AmountViewModel.Tag, AmountViewModel.Tag);
        regionManager.RequestNavigate(DetailsViewModel.Tag, DetailsViewModel.Tag);
        regionManager.RequestNavigate(HistoryViewModel.Tag, HistoryViewModel.Tag);
        regionManager.RequestNavigate(
            SettingViewModel.Tag,
            SettingViewModel.Tag,
            new NavigationParameters { { "CodeStatistic", codeStatistic } }
        );
    }

    public DelegateCommand ChangeWorkStateCommand { get; init; }

    private void ExecuteChangeWorkState()
    {
        if (!ToggleIsChecked)
        {
            ToggleIsChecked = true;
            StartStatistic();
        }
        else
        {
            StopStatistic();
        }
    }

    public DelegateCommand SaveStatisticCommand { get; init; }

    private void ExecuteSaveStatistic()
    {
        eventAggregator.GetEvent<SaveStatisticEvent>().Publish();
    }

    private void StartStatistic()
    {
        eventAggregator.GetEvent<CodeStatisticEvent>().Publish();
    }

    private void StopStatistic()
    {
        codeStatistic.StopStatistic();
    }

    private void FinishCallback()
    {
        Message = "全部文件处理完成";
        ToggleIsChecked = false;
    }

    private void CancelCallback()
    {
        Message = "操作已取消";
        ToggleIsChecked = false;
    }

    private void FileStatisticsProgress(FileStatistic fileStatistic)
    {
        Message = $"文件[{fileStatistic.FileName}]处理完成";
        eventAggregator.GetEvent<FileStatisticEvent>().Publish(fileStatistic);
    }

    private void AmountStatisticsProgress(AmountStatistics amountStatistics)
    {
        eventAggregator.GetEvent<AmountStatisticsEvent>().Publish(amountStatistics);
    }
}
