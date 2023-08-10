using System;
using Prism.Mvvm;

namespace CodeStatistics.Models;

public class StatisticSetting : BindableBase
{
    private bool chooseFolder;
    public bool ChooseFolder
    {
        get => chooseFolder;
        set => SetProperty(ref chooseFolder, value);
    }

    private bool includeSubDir;
    public bool IncludeSubDir
    {
        get => includeSubDir;
        set => SetProperty(ref includeSubDir, value);
    }

    private bool caseSensitive;
    public bool CaseSensitive
    {
        get => caseSensitive;
        set => SetProperty(ref caseSensitive, value);
    }

    private bool usingRegularity;
    public bool UsingRegularity
    {
        get => usingRegularity;
        set => SetProperty(ref usingRegularity, value);
    }
}
