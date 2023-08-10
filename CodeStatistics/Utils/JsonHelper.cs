using System.IO;
using Newtonsoft.Json;

namespace CodeStatistics.Utils;

public static class JsonHelper
{
    public static void Serialize(object value, string filePath)
    {
        using var stream = new StreamWriter(filePath);
        var json = new JsonSerializer();
        json.Serialize(stream, value);
    }

    public static TValue? Deserialize<TValue>(string filePath)
        where TValue : class
    {
        if (!File.Exists(filePath))
        {
            return default;
        }
        using var stream = new StreamReader(filePath);
        var json = new JsonSerializer();
        return json.Deserialize(stream, typeof(TValue)) as TValue;
    }
}
