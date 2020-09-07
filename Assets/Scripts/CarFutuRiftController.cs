using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Plugins.UnityChairPlugin.ChairControl.ChairWork.Options;
using Assets.Scripts;
using ChairControl.ChairWork;
using UnityEngine;

public class CarFutuRiftController : MonoBehaviour {
    private FutuRiftController controller;
    public PortShower portShower;
    void Start () {
        var comPort = EmulatorOptionsReader.ReadEmulatorOprions().comPort;
        portShower.SetPortShowing(comPort);
        int comPortNum = 1;
        if (comPort.StartsWith("COM"))
        {
            comPortNum = int.Parse(comPort.Substring(3));
        }
        controller = new FutuRiftController (new ComPortOptions { ComPort = comPortNum }, new FutuRiftOptions { interval = 50 });
        controller.Start ();
    }

    void Update () {

        var euler = transform.eulerAngles;
        controller.Pitch = (euler.x > 150 ? euler.x - 360 : euler.x);
        controller.Roll = -(euler.z > 150 ? euler.z - 360 : euler.z);
    }

    private void OnDestroy () {
        controller?.Stop ();
    }
}