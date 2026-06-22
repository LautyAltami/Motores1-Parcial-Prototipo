using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private void OnEnable()
    {
        GameManager.OnGameWin += ShowVictory;
        GameManager.OnGameLose += ShowDefeat;
    }

    private void OnDisable()
    {
        GameManager.OnGameWin -= ShowVictory;
        GameManager.OnGameLose -= ShowDefeat;
    }

    private void ShowVictory()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        UnlockMouseLook();
    }

    private void ShowDefeat()
    {
        defeatPanel.SetActive(true);
        UnlockMouseLook();
        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void UnlockMouseLook()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}