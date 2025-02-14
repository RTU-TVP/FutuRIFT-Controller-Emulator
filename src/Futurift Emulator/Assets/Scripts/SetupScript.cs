using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using UnityEngine.UI;

public class SetupScript : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown comPortsDropDown;
    [SerializeField] private TMP_InputField udpPortInput;

    [SerializeField] private Transform networkAddressesContainer;
    [SerializeField] private TMP_InputField exampleNetworkText;

    [SerializeField] private Button startEmulatorButton;
    [SerializeField] private SceneLoader sceneLoader;

    private readonly List<TMP_InputField> _networkAddresses = new();

    private void Start()
    {
        StartCoroutine(RefreshCoroutine());
        ReadOptions();
    }

    private void OnEnable()
    {
        startEmulatorButton.onClick.AddListener(SaveOptions);
        startEmulatorButton.onClick.AddListener(LoadScene);
    }

    private void OnDisable()
    {
        startEmulatorButton.onClick.RemoveListener(SaveOptions);
        startEmulatorButton.onClick.RemoveListener(LoadScene);
    }

    private void ReadOptions()
    {
        var options = EmulatorOptionsReader.ReadEmulatorOptions();

        udpPortInput.text = options.ListenUdpPortNumber.ToString();

        comPortsDropDown.value = comPortsDropDown.options
            .Select((item, index) => new { item, index })
            .SingleOrDefault(a => a.item.text == options.ComPort)
            ?.index ?? 0;
    }

    private void SaveOptions()
    {
        string comPort = null;
        if (comPortsDropDown.options.Any())
        {
            var index = comPortsDropDown.value;
            comPort = comPortsDropDown.options[index].text;
        }

        var listenUdpPortNumber = int.Parse(udpPortInput.text);

        var options = new EmulatorOptions
        {
            ComPort = comPort,
            ListenUdpPortNumber = listenUdpPortNumber
        };

        EmulatorOptionsReader.SaveEmulatorOptions(options);
    }
    
    private void LoadScene() => sceneLoader.LoadScene();

    private IEnumerator RefreshCoroutine()
    {
        var wait = new WaitForSeconds(1);

        while (true)
        {
            RefreshComPorts();
            RefreshNetworkAddresses();

            yield return wait;
        }
    }

    private void RefreshNetworkAddresses()
    {
        var ipAddresses = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .ToList();

        if (ipAddresses.All(ip => ip.ToString() != "127.0.0.1"))
        {
            ipAddresses.Add(IPAddress.Parse("127.0.0.1"));
        }

        for (var i = _networkAddresses.Count - 1; i >= ipAddresses.Count; i--)
        {
            Destroy(_networkAddresses[i].gameObject);
            _networkAddresses.RemoveAt(i);
        }

        for (var i = _networkAddresses.Count; i < ipAddresses.Count; i++)
        {
            var newText = Instantiate(exampleNetworkText, networkAddressesContainer);
            newText.gameObject.SetActive(true);
            _networkAddresses.Add(newText);
        }

        for (var i = 0; i < ipAddresses.Count; i++)
        {
            _networkAddresses[i].text = ipAddresses[i].ToString();
        }
    }

    private void RefreshComPorts()
    {
        var names = SerialPort.GetPortNames();
        comPortsDropDown.options = names.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
    }
}