using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;

public class UdpReceiver : MonoBehaviour
{
    [SerializeField] private int port = 6065;
    UdpClient Listener;
    IPEndPoint RemoteIpEndPoint;
    Thread thread;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    float pitch = 0f;
    float roll = 0f;
    private const byte escapeSymbol = 253;
    private void Start()
    {
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(pitch, transform.eulerAngles.y, roll);
    }

    private void ThreadMethod()
    {
        Listener = new UdpClient(port);
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

            byte[] receiveBytes = Listener.Receive(ref RemoteIpEndPoint);
            (pitch, roll) = ParseBytes(receiveBytes);
        }
    }

    private (float pitch, float roll) ParseBytes(byte[] receiveBytes)
    {
        // TODO Check CRC
        byte index = 4;
        var pitch = ParseFloat(receiveBytes, ref index);
        var roll = ParseFloat(receiveBytes, ref index);
        return (pitch, roll);
    }

    private float ParseFloat(byte[] data, ref byte index)
    {
        var localIndex = index;
        for (int i = 0; i < 4; i++)
        {
            if (data[index] == escapeSymbol)
            {
                index++;
                data[localIndex] = (byte)(data[index] + escapeSymbol);
            }
            else
            {
                data[localIndex] = data[index];
            }
            localIndex++;
            index++;
        }
        return BitConverter.ToSingle(data, localIndex - 4);
    }

    public void LeanForwardNBack(float n)
    {
        transform.eulerAngles = new Vector3(n, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void LeanRightNLeft(float n)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, n);
    }
}
