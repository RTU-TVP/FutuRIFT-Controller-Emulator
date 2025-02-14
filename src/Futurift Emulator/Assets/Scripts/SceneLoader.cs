using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] private bool autoLoadScene;

    private AsyncOperation _asyncOperation;

    private void Start()
    {
        _asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        _asyncOperation.allowSceneActivation = false;
        
        if (autoLoadScene)
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        _asyncOperation.allowSceneActivation = true;
    }
}