using UnityEngine;

public class InputMenu : MonoBehaviour
{
    [SerializeField] private MenuInGameManager menuManager;

    [HideInInspector] public bool isMenuOpen = false;

    void Update()
    {
        // Escucha si apretamos la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMenuOpen)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    // Esta función pausa todo y saca el mouse
    public void PauseGame()
    {
        menuManager.OpenMenu();
        isMenuOpen = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Pausa TODO el audio del juego
        AudioListener.pause = true;
    }

    // Esta función reanuda todo y esconde el mouse
    public void ResumeGame()
    {
        menuManager.CloseAll();
        isMenuOpen = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Reanuda el audio del juego
        AudioListener.pause = false;
    }
}