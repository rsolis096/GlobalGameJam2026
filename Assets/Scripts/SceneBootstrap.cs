using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrap : MonoBehaviour
{
    private string uiSceneName = "GameUI";

    void Awake()
    {
        if (!SceneManager.GetSceneByName(uiSceneName).isLoaded)
        {
            SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
        }
    }
}
