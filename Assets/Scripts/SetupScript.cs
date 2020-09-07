using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Assets.Scripts;
using System;

public class SetupScript : MonoBehaviour
{
    public TMPro.TMP_Dropdown comPortsDropDown;
    public TMP_InputField udpPortInput;


    public GameObject networkAddressesContainer;
    private List<TMP_InputField> networkAddresses = new List<TMP_InputField>();
    public TMP_InputField exampleNetworkText;
    public TextMeshProUGUI optionsFileTextLocation;
    // Start is called before the first frame update
    private void Start()
    {
        RefreshComPorts();
        ReadOptions();
        StartCoroutine(RefreshCourutine());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ReadOptions()
    {
        optionsFileTextLocation.text = EmulatorOptionsReader.OptionsFileLocation;
        var options = EmulatorOptionsReader.ReadEmulatorOprions();
        udpPortInput.text = options.listenUdpPortNumber == 0 ? "6065" : options.listenUdpPortNumber.ToString();
        comPortsDropDown.value = comPortsDropDown.options
            .Select((item, index) => new { item, index })
            .SingleOrDefault(a => a.item.text == options.comPort)
            ?.index ?? 0;

    }

    public void SaveOptions()
    {
        var options = new EmulatorOptions();
        if (comPortsDropDown.options.Any())
        {
            options.comPort = comPortsDropDown.options[comPortsDropDown.value].text;
        }
        options.listenUdpPortNumber = int.Parse(udpPortInput.text);
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

    public void RefreshNetworkAddresses()
    {
        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

        while (localIPs.Length < networkAddresses.Count)
        {
            Destroy(networkAddresses[0].gameObject);
            networkAddresses.RemoveAt(0);
        }
        while (localIPs.Length > networkAddresses.Count)
        {
            var newText = Instantiate(exampleNetworkText, networkAddressesContainer.transform);
            //newText.text = a.ToString();
            newText.gameObject.transform.localPosition -= new Vector3(0, networkAddresses.Count * 60, 0);
            newText.gameObject.SetActive(true);
            networkAddresses.Add(newText);
        }

        for (int i = 0; i < localIPs.Length; i++)
        {
            networkAddresses[i].text = localIPs[i].ToString();
        }

    }

    public void RefreshComPorts()
    {
        var names = SerialPort.GetPortNames();
        comPortsDropDown.options = names.Select(n => new TMPro.TMP_Dropdown.OptionData(n)).ToList();
    }
}
