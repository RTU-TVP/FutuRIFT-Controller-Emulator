using UnityEngine;

internal static class EmulatorOptionsReader
{
    private const string OPTIONS_PATH_KEY = "EmulatorOptionsPath";

    public static EmulatorOptions ReadEmulatorOptions()
    {
        if (!PlayerPrefs.HasKey(OPTIONS_PATH_KEY))
        {
            return new EmulatorOptions { ListenUdpPortNumber = 6065 };
        }

        var json = PlayerPrefs.GetString(OPTIONS_PATH_KEY);
        var options = JsonUtility.FromJson<EmulatorOptions>(json);
        return options ?? new EmulatorOptions { ListenUdpPortNumber = 6065 };
    }

    public static void SaveEmulatorOptions(EmulatorOptions options)
    {
        var json = JsonUtility.ToJson(options);
        PlayerPrefs.SetString(OPTIONS_PATH_KEY, json);
    }
}