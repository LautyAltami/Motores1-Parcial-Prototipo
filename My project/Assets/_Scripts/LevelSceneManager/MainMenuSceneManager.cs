using Unity.VectorGraphics;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject settingObjects;
    [SerializeField] private string levelSceneName;
    public void OpenSettings()
    {
        settingObjects.SetActive(true);
    }
    public void CloseSettings()
    {
        settingObjects.SetActive(false);
    }
    public void LoadLevel()
    {
        LevelSceneManager.Instance.LoadNameScene(levelSceneName);
    }
}
