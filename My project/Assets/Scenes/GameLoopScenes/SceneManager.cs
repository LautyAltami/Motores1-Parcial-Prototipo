using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static LevelSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void LoadWinScene()
    {
        SceneManager.LoadScene("VictoryScene");
    }
    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }
    public void LoadMainMenuScene()
    {
        Debug.Log("Loading MainMenuScene...");
        SceneManager.LoadScene("MainMenuScene");
    }
    public void LoadNameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void LoadIndexScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}