using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInGameManager : MonoBehaviour
{
    [Header("Paneles de la UI")]
    [SerializeField] private GameObject settingObject; // El panel de Opciones
    [SerializeField] private GameObject MenuInGameObject; // El panel de Pausa Principal

    [Header("Referencias Externas")]
    [SerializeField] private InputMenu input; // El script que lee el ESC

    public void OpenSettings()
    {
        settingObject.SetActive(true);
        MenuInGameObject.SetActive(false); // ¡NUEVO: Apaga la pausa para que no se superpongan!
    }

    public void CloseSettings()
    {
        settingObject.SetActive(false);
        MenuInGameObject.SetActive(true); // ¡NUEVO: Vuelve a prender la pausa al salir de opciones!
    }

    public void CloseAll()
    {
        settingObject.SetActive(false);
        MenuInGameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        MenuInGameObject.SetActive(true);
        settingObject.SetActive(false); // Nos aseguramos de que opciones arranque apagado
        Time.timeScale = 0f; // ¡BUENA PRÁCTICA: Congela el juego al pausar!
    }

    public void CloseMenu()
    {
        MenuInGameObject.SetActive(false);
        settingObject.SetActive(false); // Por si aprietan salir estando en opciones
        Time.timeScale = 1.0f;
        Debug.Log("Time Scale is 1");
        input.isMenuOpen = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1.0f;
        Debug.Log("Time Scale is 1");
        AudioListener.pause = false;
        // Usamos el Singleton seguro que ya tenías configurado
        if (LevelSceneManager.Instance != null)
        {
            LevelSceneManager.Instance.LoadMainMenuScene();
        }
    }
}