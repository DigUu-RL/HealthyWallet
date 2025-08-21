using NetJSON;
using JSON = NetJSON.NetJSON;

namespace HealthyWallet.Domain.Helpers;

public static class Json
{
    private static readonly NetJSONSettings DefaultSettings = NetJSONSettings.CurrentSettings;
    
    public static string Serialize(object value)
    {
        return JSON.SerializeObject(value, DefaultSettings);
    }
    
    public static string Serialize(object value, NetJSONSettings settings)
    {
        return JSON.SerializeObject(value, settings);
    }

    public static TResult Deserialize<TResult>(string json)
    {
        return JSON.Deserialize<TResult>(json, DefaultSettings);
    }
    
    public static TResult Deserialize<TResult>(string json, NetJSONSettings settings)
    {
        return JSON.Deserialize<TResult>(json, settings);
    }
}