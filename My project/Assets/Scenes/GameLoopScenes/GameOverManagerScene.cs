using UnityEngine;

public class GameOverManagerScene : MonoBehaviour
{
    [SerializeField] private string retryLevelSceneName;
    public void RetryScene()
    {
        LevelSceneManager.Instance.LoadNameScene(retryLevelSceneName);
    }
    public void MenuScene()
    {
        LevelSceneManager.Instance.LoadMainMenuScene();
    }
}
