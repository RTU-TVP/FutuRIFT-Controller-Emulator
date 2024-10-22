using System;

namespace Futurift.Options
{
    [Serializable]
    public class ComPortOptions
    {
        public int comPort;

        public ComPortOptions()
        {
        }

        public ComPortOptions(int comPort)
        {
            this.comPort = comPort;
        }
    }
}