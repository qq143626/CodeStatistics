using Prism.Events;

namespace CodeStatistics.ViewModels;

public class BaseViewModel
{
    protected IEventAggregator eventAggregator;

    public BaseViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }
}
