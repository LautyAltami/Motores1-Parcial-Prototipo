using UnityEngine;

public class InputMenu : MonoBehaviour
{
    [SerializeField] private MenuInGameManager menuManager;

    [HideInInspector] public bool isMenuOpen = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isMenuOpen)
            {
                menuManager.OpenMenu();
                isMenuOpen = true;
                Time.timeScale = 0f;
                Debug.Log("Time Scale is 0");
            }
            else
            {
                menuManager.CloseAll();
                isMenuOpen = false;
                Time.timeScale = 1f;
                Debug.Log("Time Scale is 1");
            }
        }
    }

}
