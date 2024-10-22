using System;

namespace Assets.Plugins.UnityChairPlugin.ChairControl.ChairWork.Options
{
    [Serializable]
    public class FutuRiftOptions
    {
        /// <summary>
        /// Interval for sending commands, in milliseconds
        /// </summary>
        public double interval = 100;

        public FutuRiftOptions()
        {
        }

        public FutuRiftOptions(double interval)
        {
            this.interval = interval;
        }
    }
}
