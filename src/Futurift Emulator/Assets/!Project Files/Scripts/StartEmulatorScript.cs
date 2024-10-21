using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEmulatorScript : MonoBehaviour
{
    [SerializeField] private string sceneName = "EmulatorScene";

    private AsyncOperation _asyncOperation;

    private void Start()
    {
        _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        if (_asyncOperation != null)
        {
            _asyncOperation.allowSceneActivation = false;
        }
        else
        {
            Debug.LogError("Failed to load scene: " + sceneName);
        }
    }

    public void StartEmulator()
    {
        _asyncOperation.allowSceneActivation = true;
    }
}