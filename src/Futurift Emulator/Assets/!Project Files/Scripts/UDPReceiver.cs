using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private UdpClient _listener;
    private IPEndPoint _remoteIpEndPoint;

    private Thread _thread;

    private float _pitch = -15f;
    private float _roll = 0f;
    private const byte ESCAPE_SYMBOL = 253;
    private (float, float) _defaultPosition = (-15f, 0f);
    private DateTime _received;

    private void Start()
    {
        _thread = new Thread(ThreadMethod);
        _thread.Start();
        StartCoroutine(ResetToZero());
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _listener.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            (_pitch, _roll) = _defaultPosition;
        }

        LeanForwardNBack(_pitch);
        LeanRightNLeft(_roll);
    }

    private void ThreadMethod()
    {
        var port = EmulatorOptionsReader.ReadEmulatorOprions().ListenUdpPortNumber;
        _listener = new UdpClient(port);
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            _remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            try
            {
                var receiveBytes = _listener.Receive(ref _remoteIpEndPoint);
                if (receiveBytes.Length == 0)
                {
                    continue;
                }

                _received = DateTime.UtcNow;
                (_pitch, _roll) = ParseBytes(receiveBytes);
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
        for (var i = 0; i < 4; i++)
        {
            if (data[index] == ESCAPE_SYMBOL)
            {
                index++;
                data[localIndex] = (byte)(data[index] + ESCAPE_SYMBOL);
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
            if (DateTime.UtcNow - _received > waitDelay)
            {
                (_pitch, _roll) = _defaultPosition;
            }
        }
    }

    public void LeanForwardNBack(float n)
    {
        transform.localEulerAngles = new Vector3(n, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void LeanRightNLeft(float n)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, n);
    }
}