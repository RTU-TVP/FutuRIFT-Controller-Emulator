using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] private bool autoLoadNextScene = true;

    private AsyncOperation _asyncOperation;

    private void Start()
    {
        LoadNextSceneAsync();
        
        if (autoLoadNextScene)
        {
            LoadNextSceneManually();
        }
    }
    
    private void LoadNextSceneAsync()
    {
        var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        _asyncOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        _asyncOperation.allowSceneActivation = false;
    }
    
    public void LoadNextSceneManually()
    {
        _asyncOperation.allowSceneActivation = true;
    }
}