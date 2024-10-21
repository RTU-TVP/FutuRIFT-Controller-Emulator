using Assets.Plugins.UnityChairPlugin.ChairControl.ChairWork.Options;
using ChairControl.ChairWork;
using UnityEngine;

public class CarFutuRiftController : MonoBehaviour
{
    [SerializeField] private PortShower portShower;

    private FutuRiftController _controller;

    private void Start()
    {
        var comPort = EmulatorOptionsReader.ReadEmulatorOprions().ComPort;

        portShower.SetPortShowing(comPort);

        var comPortNum = 1;
        if (comPort.StartsWith("COM"))
        {
            comPortNum = int.Parse(comPort[3..]);
        }

        _controller = new FutuRiftController(new ComPortOptions(comPortNum), new FutuRiftOptions(50));
        _controller.Start();
    }

    private void Update()
    {
        if (_controller == null)
        {
            return;
        }
        
        var euler = transform.eulerAngles;
        _controller.Pitch = (euler.x > 150 ? euler.x - 360 : euler.x);
        _controller.Roll = -(euler.z > 150 ? euler.z - 360 : euler.z);
    }

    private void OnDestroy()
    {
        _controller?.Stop();
    }
}