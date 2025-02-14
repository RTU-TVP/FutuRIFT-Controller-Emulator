using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

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

    public void LoadScene()
    {
        _asyncOperation.allowSceneActivation = true;
    }
}