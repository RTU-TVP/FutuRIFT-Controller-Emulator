namespace Assets.Plugins.UnityChairPlugin.ChairControl.ChairWork
{
    internal interface IDataSender
    {
        void SendData(byte[] data);
        void Start();
        void Stop();
    }
}
