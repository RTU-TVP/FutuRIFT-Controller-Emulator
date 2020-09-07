using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEmulatorScript : MonoBehaviour
{
    public void StartEmulator()
    {
        SceneManager.LoadScene("EmulatorScene");
    }
}
