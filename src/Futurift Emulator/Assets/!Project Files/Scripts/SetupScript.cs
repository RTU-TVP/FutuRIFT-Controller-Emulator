using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class SetupScript : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown comPortsDropDown;
    [SerializeField] private TMP_InputField udpPortInput;

    [SerializeField] private Transform networkAddressesContainer;
    [SerializeField] private TMP_InputField exampleNetworkText;

    [SerializeField] private TextMeshProUGUI optionsFileTextLocation;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button refreshComPortsButton;
    [SerializeField] private Button refreshNetworkAddressesButton;

    private readonly List<TMP_InputField> _networkAddresses = new();

    private void Start()
    {
        RefreshComPorts();
        ReadOptions();
        StartCoroutine(RefreshCourutine());

        // RefreshComPorts();
        // RefreshNetworkAddresses();
    }

    private void OnEnable()
    {
        saveButton.onClick.AddListener(SaveOptions);
        refreshComPortsButton.onClick.AddListener(RefreshComPorts);
        refreshNetworkAddressesButton.onClick.AddListener(RefreshNetworkAddresses);
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveListener(SaveOptions);
        refreshComPortsButton.onClick.RemoveListener(RefreshComPorts);
        refreshNetworkAddressesButton.onClick.RemoveListener(RefreshNetworkAddresses);
    }

    private void ReadOptions()
    {
        optionsFileTextLocation.text = EmulatorOptionsReader.OptionsFileLocation;

        var options = EmulatorOptionsReader.ReadEmulatorOprions();
        udpPortInput.text = options.ListenUdpPortNumber == 0 ? "6065" : options.ListenUdpPortNumber.ToString();

        comPortsDropDown.value = comPortsDropDown.options
            .Select((item, index) => new { item, index })
            .SingleOrDefault(a => a.item.text == options.ComPort)
            ?.index ?? 0;
    }

    private void SaveOptions()
    {
        var options = new EmulatorOptions();

        if (comPortsDropDown.options.Any())
        {
            options.ComPort = comPortsDropDown.options[comPortsDropDown.value].text;
        }

        options.ListenUdpPortNumber = int.Parse(udpPortInput.text);

        var optionsJson = JsonUtility.ToJson(options);
        File.WriteAllText(EmulatorOptionsReader.OptionsFileLocation, optionsJson);
    }

    private IEnumerator RefreshCourutine()
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
        var localIPs = Dns.GetHostAddresses(Dns.GetHostName());

        while (localIPs.Length < _networkAddresses.Count)
        {
            Destroy(_networkAddresses[0].gameObject);
            _networkAddresses.RemoveAt(0);
        }

        while (localIPs.Length > _networkAddresses.Count)
        {
            var newText = Instantiate(exampleNetworkText, networkAddressesContainer);
            newText.gameObject.SetActive(true);
            _networkAddresses.Add(newText);
        }

        for (var i = 0; i < localIPs.Length; i++)
        {
            _networkAddresses[i].text = localIPs[i].ToString();
        }
    }

    private void RefreshComPorts()
    {
        var names = SerialPort.GetPortNames();
        comPortsDropDown.options = names.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
    }
}