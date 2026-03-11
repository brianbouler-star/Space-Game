using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "MainScene";

    private void Start()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
