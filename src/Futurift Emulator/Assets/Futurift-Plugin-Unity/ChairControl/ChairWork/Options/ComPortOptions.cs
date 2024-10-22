using System;

namespace Assets.Plugins.UnityChairPlugin.ChairControl.ChairWork.Options
{
    [Serializable]
    public class ComPortOptions
    {
        public int ComPort;

        public ComPortOptions()
        {
        }

        public ComPortOptions(int comPort)
        {
            ComPort = comPort;
        }
    }
}
