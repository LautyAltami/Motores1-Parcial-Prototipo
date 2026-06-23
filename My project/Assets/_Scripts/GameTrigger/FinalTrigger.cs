using UnityEngine;
using System.Collections;


public class FinalTrigger : MonoBehaviour
{

    [Header("Configuración del Final")]

    [SerializeField] private GameObject panelNegroUI; // El panel negro del Canvas

    [SerializeField] private float tiempoEnNegro = 2f; // Segundos de espera



    private bool yaSeActivo = false;



    private void OnTriggerEnter(Collider other)

    {

        // Detecta si lo que entró al trigger es el jugador y evita que se dispare dos veces

        if (other.CompareTag("Player") && !yaSeActivo)

        {

            yaSeActivo = true;

            StartCoroutine(SecuenciaFinal());
        }

    }

    private IEnumerator SecuenciaFinal()
    {
        // 1. Activamos el panel negro en la pantalla instantáneamente

        if (panelNegroUI != null)

        {
            panelNegroUI.SetActive(true);

        }

        // 2.Esperamos los segundos configurados en la oscuridad

        yield return new WaitForSeconds(tiempoEnNegro);

        // 3. Llamamos al SceneManager global para viajar a la escena del hospital

        if (LevelSceneManager.Instance != null)

        {

            LevelSceneManager.Instance.LoadNameScene("VictoryScene");

        }

        else

        {

            Debug.LogError("¡Ojo! No se encontró el LevelSceneManager en la escena.");

        }

    }
}