using System.Windows;
using System.Windows.Controls;

namespace CodeStatistics.Utils;

public static class DictionaryResource
{
    public static string GetString(string resourceKey)
    {
        return (string)Application.Current.Resources[resourceKey];
    }

    public static Style GetStyle(string resourceKey)
    {
        return (Style)Application.Current.Resources[resourceKey];
    }
}
