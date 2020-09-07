using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class EmulatorOptionsReader
    {
        public static string OptionsFileLocation => Path.Combine(Directory.GetCurrentDirectory(), "options.json");

        public static EmulatorOptions ReadEmulatorOprions()
        {
            if (!File.Exists(OptionsFileLocation))
            {
                return new EmulatorOptions { listenUdpPortNumber = 6065 };
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
}
