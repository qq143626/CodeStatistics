using System;

namespace CodeStatistics.Utils;

static class Constant
{
    public static string Root { get; } = Environment.CurrentDirectory;

    public static string SettingDirectory { get; } = $"{Root}/Setting";

    public static string FileTypeFile { get; } = $"{SettingDirectory}/FileType.json";

    public static string StatisticSettingFile { get; } =
        $"{SettingDirectory}/StatisticSetting.json";

    public static string HistoryDirectory { get; } = $"{Root}/History";

    public static string HistoryStatistics { get; } = $"{HistoryDirectory}/{{0}}_{{1:000}}.json";
}
