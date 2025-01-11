using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private int sceneIndex = 1;
    
    private void Start()
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}