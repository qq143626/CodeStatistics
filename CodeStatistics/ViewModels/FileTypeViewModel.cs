using System;
using System.Collections.Generic;
using System.Linq;
using CodeStatistics.Models;
using CodeStatistics.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace CodeStatistics.ViewModels;

public class FileTypeViewModel : BindableBase, IDialogAware
{
    public const string Tag = "FileTypeDialog";

    private string? codeType;
    public string CodeType
    {
        get => codeType ?? string.Empty;
        set => SetProperty(ref codeType, value);
    }

    private string? extension;
    public string Extension
    {
        get => extension ?? string.Empty;
        set => SetProperty(ref extension, value);
    }

    private string? singleComments;
    public string SingleComments
    {
        get => singleComments ?? string.Empty;
        set => SetProperty(ref singleComments, value);
    }

    private string? multiCommentsStart;
    public string MultiCommentsStart
    {
        get => multiCommentsStart ?? string.Empty;
        set => SetProperty(ref multiCommentsStart, value);
    }

    private string? multiCommentsEnd;
    public string MultiCommentsEnd
    {
        get => multiCommentsEnd ?? string.Empty;
        set => SetProperty(ref multiCommentsEnd, value);
    }

    public List<FileType> FileTypeDatas { get; init; }
    public List<string> CodeTypeDatas { get; init; }

    public event Action<IDialogResult>? RequestClose;

    public FileTypeViewModel()
    {
        var fileTypeDatas = JsonHelper.Deserialize<List<FileType>>(
            StorageManager.GetFileTypeFile()
        );
        FileTypeDatas = fileTypeDatas ??= new();
        CodeTypeDatas = FileTypeDatas.Select(x => x.CodeType).ToList();

        SelectedItemChangedCommand = new(ExecuteSelectedItemChanged);
        SubmitFileTypeCommand = new(ExecuteSubmitFileType);
        CancelFileTypeCommand = new(ExecuteCancelFileType);
    }

    public DelegateCommand<int?> SelectedItemChangedCommand { get; init; }

    private void ExecuteSelectedItemChanged(int? selectedIndex)
    {
        if (selectedIndex is null or < 0)
        {
            return;
        }
        int index = selectedIndex ?? 0;
        var fileType = FileTypeDatas[index];
        if (fileType != null)
        {
            CodeType = fileType.CodeType;
            Extension = fileType.Extension;
            SingleComments = fileType.SingleComments;
            MultiCommentsStart = fileType.MultiCommentsStart;
            MultiCommentsEnd = fileType.MultiCommentsEnd;
        }
    }

    public DelegateCommand SubmitFileTypeCommand { get; init; }

    private void ExecuteSubmitFileType()
    {
        FileType fileType =
            new()
            {
                Enabled = true,
                CodeType = CodeType,
                Extension = Extension,
                SingleComments = SingleComments,
                MultiCommentsStart = MultiCommentsStart,
                MultiCommentsEnd = MultiCommentsEnd,
            };
        AddFileType(fileType);
        RequestClose?.Invoke(
            new DialogResult(ButtonResult.OK, new DialogParameters { { "fileType", fileType } })
        );
    }

    private void AddFileType(FileType fileType)
    {
        FileTypeDatas.RemoveAll(x => x.CodeType == fileType.CodeType);

        FileTypeDatas.Add(fileType);

        JsonHelper.Serialize(FileTypeDatas, StorageManager.GetFileTypeFile());
    }

    public DelegateCommand CancelFileTypeCommand { get; init; }

    private void ExecuteCancelFileType()
    {
        RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
    }

    public string Title => DictionaryResource.GetString("AddFileType");

    public bool CanCloseDialog() => true;

    public void OnDialogClosed() { }

    public void OnDialogOpened(IDialogParameters parameters) { }
}
