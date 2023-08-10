using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeStatistics.Utils;

internal static class StorageManager
{
    public static string HistoryDirectory()
    {
        CreateDirectory(Constant.HistoryDirectory);
        return Constant.HistoryDirectory;
    }

    public static string GetHistoryStatistics()
    {
        CreateDirectory(Constant.HistoryDirectory);
        var date = DateTime.Now.ToString("yyyy_MM_dd");
        var fileNames = Directory.GetFiles(Constant.HistoryDirectory, $"{date}_*.json");
        var index = 1;
        foreach (var fileName in fileNames)
        {
            var group = Regex.Match(fileName, $"{date}_(\\d+)\\.json").Groups[1];
            if (group.Success)
            {
                var tmp = int.Parse(group.Value);
                if (tmp >= index)
                {
                    index = tmp + 1;
                }
            }
        }

        return string.Format(Constant.HistoryStatistics, date, index);
    }

    public static string[] GetAllHistoryStatisticsFile()
    {
        CreateDirectory(Constant.HistoryDirectory);
        return Directory.GetFiles(Constant.HistoryDirectory, "*.json");
    }

    public static string GetFileTypeFile()
    {
        CreateDirectory(Constant.SettingDirectory);
        return Constant.FileTypeFile;
    }

    public static string GetStatisticSettingFile()
    {
        CreateDirectory(Constant.SettingDirectory);
        return Constant.StatisticSettingFile;
    }

    private static void CreateDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
