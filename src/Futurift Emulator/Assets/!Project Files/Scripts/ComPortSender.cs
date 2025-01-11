using System.IO.Ports;
using FutuRIFT;

internal class ComPortSender : ISender
{
    public bool IsConnected => _port.IsOpen;

    private readonly SerialPort _port;

    public ComPortSender(int comPort)
    {
        _port = new SerialPort
        {
            BaudRate = 115200,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            ReadBufferSize = 4096,
            WriteBufferSize = 4096,
            ReadTimeout = 500,
            PortName = $"COM{comPort}",
        };
    }

    public void Connect()
    {
        _port.Open();
    }

    public void Disconnect()
    {
        _port.Close();
    }

    public void Send(byte[] data)
    {
        _port.Write(data, 0, data.Length);
    }
}