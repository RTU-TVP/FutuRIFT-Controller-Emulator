using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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
        _ = ResetToZeroAsync(_cancellationTokenSource.Token);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _listener?.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetPitchAndRoll();
        }

        AdjustPitch(_pitch);
        AdjustRoll(_roll);
    }

    private void ResetPitchAndRoll()
    {
        (_pitch, _roll) = _defaultPosition;
    }

    private async void ThreadMethod()
    {
        var port = EmulatorOptionsReader.ReadEmulatorOptions().ListenUdpPortNumber;
        _listener = new UdpClient(port);
        _remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var result = await _listener.ReceiveAsync(); // Используем асинхронный метод
                _received = DateTime.UtcNow;
                (_pitch, _roll) = ParseBytes(result.Buffer);
            }
            catch (SocketException ex)
            {
                Debug.LogError($"Socket error: {ex.Message}");
            }
            catch (ObjectDisposedException)
            {
                Debug.Log("Connection closed");
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error: {ex.Message}");
            }
        }
        
        Debug.Log("ThreadMethod finished");
        
        _listener.Close();
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
        var floatBytes = new byte[4];
        for (var i = 0; i < 4; i++)
        {
            if (data[index] == ESCAPE_SYMBOL)
            {
                index++;
                floatBytes[i] = (byte)(data[index] + ESCAPE_SYMBOL);
            }
            else
            {
                floatBytes[i] = data[index];
            }
            index++;
        }

        return BitConverter.ToSingle(floatBytes, 0);
    }

    private async Task ResetToZeroAsync(CancellationToken cancellationToken)
    {
        var waitDelay = TimeSpan.FromSeconds(3);
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
            if (DateTime.UtcNow - _received > waitDelay)
            {
                (_pitch, _roll) = _defaultPosition;
            }
        }
    }

    private void AdjustPitch(float n)
    {
        transform.localEulerAngles = new Vector3(n, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    private void AdjustRoll(float n)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, n);
    }
}

internal class EmulatorOptions
{
    public string ComPort;
    public int ListenUdpPortNumber;
}