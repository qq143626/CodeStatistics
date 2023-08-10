using System.Diagnostics;
using CodeStatistics.Models;
using CodeStatistics.Models.Events;
using Prism.Events;

namespace CodeStatistics.ViewModels;

public class AmountViewModel : BaseViewModel
{
    public const string Tag = "AmountRegion";

    public AmountStatistics AmountStatistics { get; init; }

    public AmountViewModel(IEventAggregator eventAggregator)
        : base(eventAggregator)
    {
        AmountStatistics = new();

        eventAggregator.GetEvent<AmountStatisticsEvent>().Subscribe(AmountStatisticsChanged);
        eventAggregator.GetEvent<SaveStatisticReadyEvent>().Subscribe(SaveAmountStatistics);
    }

    private void AmountStatisticsChanged(AmountStatistics amountStatistics)
    {
        amountStatistics.AssignTo(AmountStatistics);
    }

    private void SaveAmountStatistics()
    {
        eventAggregator.GetEvent<AmountStatisticsSaveEvent>().Publish(AmountStatistics);
    }
}
