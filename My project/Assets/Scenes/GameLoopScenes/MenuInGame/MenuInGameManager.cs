using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInGameManager : MonoBehaviour
{
    [SerializeField] private GameObject settingObject;
    [SerializeField] private GameObject MenuInGameObject;
    [SerializeField] private InputMenu input;
    public void OpenSettings()
    {
        settingObject.SetActive(true);
    }
    public void CloseSettings()
    {
        settingObject.SetActive(false);
    }
    public void CloseAll()
    {
        settingObject.SetActive(false);
        MenuInGameObject.SetActive(false);
    }
    public void OpenMenu()
    {
        MenuInGameObject.SetActive(true);
    }
    public void CloseMenu()
    {
        MenuInGameObject.SetActive(false);
        Time.timeScale = 1.0f;
        Debug.Log("Time Scale is 1");
        input.isMenuOpen = false;
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1.0f;
        Debug.Log("Time Scale is 1");
        LevelSceneManager.Instance.LoadMainMenuScene();
    }

}
