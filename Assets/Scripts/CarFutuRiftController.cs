using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Plugins.UnityChairPlugin.ChairControl.ChairWork.Options;
using ChairControl.ChairWork;
using UnityEngine;

public class CarFutuRiftController : MonoBehaviour {
    private FutuRiftController controller;
    public ComPortOptions comPortOptions;
    public PortShower portShower;
    void Start () {
        try
        {
            var filestr = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "port.txt"));
            Debug.LogError(filestr);
            comPortOptions.ComPort = int.Parse(filestr);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        portShower.SetPortShowing(comPortOptions.ComPort);
        controller = new FutuRiftController (comPortOptions, new FutuRiftOptions { interval = 50 });
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