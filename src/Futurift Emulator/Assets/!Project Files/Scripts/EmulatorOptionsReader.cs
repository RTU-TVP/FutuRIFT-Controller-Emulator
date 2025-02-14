using System;
using System.IO;
using UnityEngine;

internal class EmulatorOptionsReader
{
    public static string OptionsFileLocation => Path.Combine(Application.persistentDataPath, "options.json");

    public static EmulatorOptions ReadEmulatorOptions()
    {
        if (!File.Exists(OptionsFileLocation))
        {
            return new EmulatorOptions { ListenUdpPortNumber = 6065 };
        }

        try
        {
            var text = File.ReadAllText(OptionsFileLocation);
            var options = JsonUtility.FromJson<EmulatorOptions>(text);
            return options ?? new EmulatorOptions { ListenUdpPortNumber = 6065 };
        }
        catch (Exception ex)
        {
            Debug.LogError($"Can't read options file: {ex.Message}");
            return new EmulatorOptions { ListenUdpPortNumber = 6065 };
        }
    }
    
    public static void SaveEmulatorOptions(EmulatorOptions options)
    {
        try
        {
            var text = JsonUtility.ToJson(options);
            File.WriteAllText(OptionsFileLocation, text);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Can't save options file: {ex.Message}");
        }
    }
}