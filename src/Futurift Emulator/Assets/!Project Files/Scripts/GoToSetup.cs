using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSetup : MonoBehaviour
{
    public void OpenSetup()
    {
        SceneManager.LoadScene("SetupScene");
    }
}