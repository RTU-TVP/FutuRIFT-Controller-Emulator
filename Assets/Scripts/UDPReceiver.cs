using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    UdpClient Listener;
    IPEndPoint RemoteIpEndPoint;
    Thread thread;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    float pitch = -15f;
    float roll = 0f;
    private const byte escapeSymbol = 253;
    private (float, float) defaultPosition = (-15f, 0f);
    private DateTime received;

    private void Start()
    {
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
        StartCoroutine(ResetToZero());
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        Listener.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            (pitch, roll) = defaultPosition;
        }
        transform.eulerAngles = new Vector3(pitch, transform.eulerAngles.y, roll);
    }

    private void ThreadMethod()
    {
        var port = EmulatorOptionsReader.ReadEmulatorOprions().listenUdpPortNumber;
        Listener = new UdpClient(port);
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            try
            {
                byte[] receiveBytes = Listener.Receive(ref RemoteIpEndPoint);
                if (receiveBytes.Length == 0)
                {
                    continue;
                }
                received = DateTime.UtcNow;
                (pitch, roll) = ParseBytes(receiveBytes);
            }
            catch (SocketException sex) when (sex.SocketErrorCode == SocketError.Interrupted)
            {
                Debug.Log("Connection closed");
            }
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

    private IEnumerator ResetToZero()
    {
        var wait = new WaitForSeconds(1);
        var waitDelay = TimeSpan.FromSeconds(3);
        while (true)
        {
            yield return wait;
            if (DateTime.UtcNow - received > waitDelay)
            {
                (pitch, roll) = defaultPosition;
            }
        }
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