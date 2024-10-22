using System;
using System.IO;
using UnityEngine;

internal class EmulatorOptionsReader
{
    public static string OptionsFileLocation => Path.Combine(Directory.GetCurrentDirectory(), "options.json");

    public static EmulatorOptions ReadEmulatorOprions()
    {
        if (!File.Exists(OptionsFileLocation))
        {
            return new EmulatorOptions { ListenUdpPortNumber = 6065 };
        }

        try
        {
            var text = File.ReadAllText(OptionsFileLocation);
            var options = JsonUtility.FromJson<EmulatorOptions>(text);
            return options;
        }
        catch (Exception ex)
        {
            Debug.LogError("Can't read options file " + ex.Message);
            throw;
        }
    }
}