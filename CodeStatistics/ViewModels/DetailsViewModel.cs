using System.Collections.ObjectModel;
using CodeStatistics.Models;
using CodeStatistics.Models.Events;
using Prism.Events;

namespace CodeStatistics.ViewModels;

public class DetailsViewModel : BaseViewModel
{
    public const string Tag = "DetailsRegion";

    public ObservableCollection<FileStatistic> FileStatisticDatas { get; init; }

    public DetailsViewModel(IEventAggregator eventAggregator)
        : base(eventAggregator)
    {
        FileStatisticDatas = new();

        eventAggregator.GetEvent<CodeStatisticEvent>().Subscribe(() => FileStatisticDatas.Clear());
        eventAggregator.GetEvent<FileStatisticEvent>().Subscribe(FileStatisticAdd);
        eventAggregator.GetEvent<SaveStatisticReadyEvent>().Subscribe(SaveFileStatistic);
    }

    private void FileStatisticAdd(FileStatistic fileStatistic)
    {
        App.PropertyChangeAsync(() => FileStatisticDatas.Add(fileStatistic));
    }

    private void SaveFileStatistic()
    {
        eventAggregator.GetEvent<FileStatisticSaveEvent>().Publish(FileStatisticDatas);
    }
}
