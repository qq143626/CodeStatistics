using System;
using System.Collections.ObjectModel;
using System.Windows;
using CodeStatistics.Models;
using CodeStatistics.ViewModels;
using CodeStatistics.Views;
using Prism.DryIoc;
using Prism.Ioc;

namespace CodeStatistics;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    public static ObservableCollection<FileStatistic>? FileStatisticDatas { get; private set; }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<AmountView>(AmountViewModel.Tag);
        containerRegistry.RegisterForNavigation<DetailsView>(DetailsViewModel.Tag);
        containerRegistry.RegisterForNavigation<HistoryView>(HistoryViewModel.Tag);
        containerRegistry.RegisterForNavigation<SettingView>(SettingViewModel.Tag);

        containerRegistry.RegisterDialog<FileTypeView>(FileTypeViewModel.Tag);
    }

    public static void PropertyChangeAsync(Action callback)
    {
        if (Current == null)
        {
            return;
        }

        Current.Dispatcher.Invoke(callback);
    }
}
