using UnityEngine;

public class PortShower : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMesh;

    public void SetPortShowing(string comPort)
    {
        textMesh.text = comPort;
    }
}