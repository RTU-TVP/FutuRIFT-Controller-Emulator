using FutuRIFT;
using UnityEngine;

public class CarFutuRIFTController : MonoBehaviour
{
    private FutuRIFTController _controller;
    private ComPortSender _comPortSender;
    
    private void Start()
    {
        var comPort = EmulatorOptionsReader.ReadEmulatorOptions().ComPort;

        var comPortNum = 1;
        if (comPort != null && comPort.StartsWith("COM"))
        {
            comPortNum = int.Parse(comPort[3..]);
        }

        _comPortSender = new ComPortSender(
            comPortNum
        );
        
        _controller = new FutuRIFTController(
            _comPortSender
        );

        _comPortSender?.Connect();
        _controller?.Start();
    }

    private void Update()
    {
        if (_controller == null)
        {
            return;
        }

        var euler = transform.localEulerAngles;
        _controller.Pitch = euler.x > 180 ? euler.x - 360 : euler.x;
        _controller.Roll = -(euler.z > 180 ? euler.z - 360 : euler.z);
    }

    private void OnDestroy()
    {
        _comPortSender?.Disconnect();
        _controller?.Stop();
    }
}