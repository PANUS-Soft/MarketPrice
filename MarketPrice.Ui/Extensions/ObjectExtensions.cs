namespace MarketPrice.Ui.Extensions;

public static class ObjectExtensions
{
    // create serialize and deserialize methods for objects using System.Text.Json
    public static string ToJson(this object obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }

    public static T? FromJson<T>(this string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }
}