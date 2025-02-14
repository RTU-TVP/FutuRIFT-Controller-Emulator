using FutuRIFT;
using UnityEngine;

public class CarFutuRIFTController : MonoBehaviour
{
    private FutuRIFTController _controller;

    private void Start()
    {
        var comPort = EmulatorOptionsReader.ReadEmulatorOptions().ComPort;

        var comPortNum = 1;
        if (comPort != null && comPort.StartsWith("COM"))
        {
            comPortNum = int.Parse(comPort[3..]);
        }
        
        Debug.Log($"Using COM port: {comPortNum}");

        _controller = new FutuRIFTController(
            new ComPortSender(
                comPortNum
            )
        );

        _controller.Start();
    }

    private void Update()
    {
        if (_controller == null)
        {
            return;
        }

        var euler = transform.eulerAngles;
        _controller.Pitch = euler.x > 180 ? euler.x - 360 : euler.x;
        _controller.Roll = -(euler.z > 180 ? euler.z - 360 : euler.z);
    }

    private void OnDestroy()
    {
        _controller?.Stop();
    }
}