using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CodeStatistics.Models;
using CodeStatistics.Models.Events;
using CodeStatistics.Utils;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace CodeStatistics.ViewModels;

public class SettingViewModel : BaseViewModel, INavigationAware
{
    public const string Tag = "SettingRegion";

    public StatisticSetting StatisticSetting { get; private set; }
    public ObservableCollection<CodeDirectory> CodeDirectoryDatas { get; init; }
    public ObservableCollection<FileType> FileTypeDatas { get; init; }
    public ObservableCollection<SearchKeyword> SearchKeywordDatas { get; init; }

    private CodeStatistic? codeStatistic;

    private IDialogService dialogService;

    public SettingViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        : base(eventAggregator)
    {
        this.dialogService = dialogService;

        CodeDirectoryDatas = new();
        FileTypeDatas = new();
        SearchKeywordDatas = new();

        var statisticSetting = JsonHelper.Deserialize<StatisticSetting>(
            StorageManager.GetStatisticSettingFile()
        );
        StatisticSetting = statisticSetting ??= new();

        AddCodeDirCommand = new(ExecuteAddCodeDir);
        AddFileTypeCommand = new(ExecuteAddFileType);
        AddSearchKeywordCommand = new(ExecuteAddSearchKeyword);

        eventAggregator.GetEvent<CodeStatisticEvent>().Subscribe(CodeStatisticSetting);
        eventAggregator.GetEvent<SaveStatisticReadyEvent>().Subscribe(SaveStatisticSetting);
    }

    public DelegateCommand AddCodeDirCommand { get; init; }

    private void ExecuteAddCodeDir()
    {
        string filter = string.Empty;
        if (FileTypeDatas.Count > 0)
        {
            StringBuilder builder = new();
            filter = string.Join("|", FileTypeDatas.Select(x => FileTypeFilter(x)));
        }
        OpenFileDialog openFileDialog =
            new()
            {
                Title = "选择源码文件",
                Filter = filter,
                RestoreDirectory = true,
            };
        if (openFileDialog.ShowDialog() == true)
        {
            string fileName = openFileDialog.FileName;
            if (StatisticSetting?.ChooseFolder == true)
            {
                fileName = Path.GetDirectoryName(fileName) ?? string.Empty;
            }

            if (CodeDirectoryDatas.Any(x => x.Directory == fileName))
            {
                return;
            }
            CodeDirectoryDatas.Add(new CodeDirectory { Enabled = true, Directory = fileName, });
        }
    }

    private string FileTypeFilter(FileType fileType)
    {
        return $"{fileType.CodeType}|{fileType.Extension}";
    }

    public DelegateCommand AddFileTypeCommand { get; init; }

    private void ExecuteAddFileType()
    {
        dialogService.ShowDialog(
            FileTypeViewModel.Tag,
            result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var fileType = result.Parameters.GetValue<FileType>("fileType");
                    if (FileTypeDatas.Any(x => x.Extension == fileType.Extension))
                    {
                        return;
                    }
                    FileTypeDatas.Add(fileType);
                }
            }
        );
    }

    public DelegateCommand AddSearchKeywordCommand { get; init; }

    private void ExecuteAddSearchKeyword()
    {
        SearchKeywordDatas.Add(new SearchKeyword { Enabled = true, Keyword = string.Empty });
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;

    public void OnNavigatedFrom(NavigationContext navigationContext) { }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        codeStatistic = navigationContext.Parameters.GetValue<CodeStatistic>("CodeStatistic");
    }

    private void CodeStatisticSetting()
    {
        SerializeStatisticSetting();

        var directories = CodeDirectoryDatas
            .Where(x => x.Enabled)
            .Select(x => x.Directory)
            .ToList();
        var fileTypeList = FileTypeDatas.Where(x => x.Enabled).ToList();
        var includeSubDir = StatisticSetting?.IncludeSubDir ?? true;
        codeStatistic?.StatisticFilesSetting(directories, fileTypeList, includeSubDir);

        if (SearchKeywordDatas.Count > 0)
        {
            var keywords = SearchKeywordDatas.Where(x => x.Enabled).Select(x => x.Keyword).ToList();
            var caseSensitive = StatisticSetting?.CaseSensitive ?? true;
            var usingRegularity = StatisticSetting?.UsingRegularity ?? true;
            codeStatistic?.StatisticKeywordsSetting(keywords, caseSensitive, usingRegularity);
        }

        codeStatistic?.StartStatistic();
    }

    private void SerializeStatisticSetting()
    {
        JsonHelper.Serialize(StatisticSetting, StorageManager.GetStatisticSettingFile());
    }

    private void SaveStatisticSetting()
    {
        eventAggregator
            .GetEvent<SettingSetSaveEvent>()
            .Publish((CodeDirectoryDatas, FileTypeDatas, SearchKeywordDatas));
    }
}
