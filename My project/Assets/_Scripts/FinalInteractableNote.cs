using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class FinalInteractableNote : ObjetoInteractivoBase
{
    [Header("UI de la Nota")]
    [Tooltip("El Canvas que ya tienes armado con la imagen de la hoja")]
    public GameObject canvasNota;
    public string uiText = "Leer nota final";

    [Header("Configuración del Cierre de Juego")]
    public float tiempoLectura = 10f; // Los 10 segundos para leer
    [Tooltip("Nombre exacto de la escena a la que irá (ej. 'MainMenu' o 'Credits')")]
    public string nombreEscenaDestino = "MainMenu";

    [Header("Fade a Negro")]
    [Tooltip("Arrastra una Imagen negra que ocupe toda la pantalla (dentro de tu Canvas de UI)")]
    public Image imageFadeNegro;
    public float duracionFade = 2f; // Cuánto tarda en ponerse todo negro

    [Header("Audio")]
    public AudioClip grabSound;

    private AudioSource audioSource;
    private bool yaInteractuo = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (canvasNota != null)
            canvasNota.SetActive(false);

        // Nos aseguramos de que la imagen de fade empiece totalmente invisible
        if (imageFadeNegro != null)
        {
            imageFadeNegro.gameObject.SetActive(true);
            Color c = imageFadeNegro.color;
            c.a = 0f;
            imageFadeNegro.color = c;
        }
    }

    public override string ObtenerMensaje()
    {
        if (yaInteractuo) return "";
        return uiText;
    }

    public override void Interact(GameObject player)
    {
        if (yaInteractuo) return;
        yaInteractuo = true;

        // Iniciamos la secuencia final del juego
        StartCoroutine(SecuenciaFinalJuego());
    }

    IEnumerator SecuenciaFinalJuego()
    {
        // 1. Mostrar la nota en pantalla y reproducir sonido
        if (canvasNota != null) canvasNota.SetActive(true);
        if (grabSound != null) audioSource.PlayOneShot(grabSound);

        // 2. Esperar los 10 segundos que tiene el jugador para leer
        yield return new WaitForSeconds(tiempoLectura);

        // 3. Hacer el Fade a Negro (Lerp del canal Alpha)
        if (imageFadeNegro != null)
        {
            float tiempoElapsado = 0f;
            while (tiempoElapsado < duracionFade)
            {
                tiempoElapsado += Time.deltaTime;
                float alpha = Mathf.Clamp01(tiempoElapsado / duracionFade);

                Color c = imageFadeNegro.color;
                c.a = alpha;
                imageFadeNegro.color = c;

                yield return null; // Espera al siguiente frame
            }
        }
        else
        {
            // Si por alguna razón te olvidaste de asignar la imagen, espera 2 segundos igual antes de cambiar de escena
            yield return new WaitForSeconds(2f);
        }

        // 4. Romper el loop y cargar la siguiente escena (Menú o Créditos)
        SceneManager.LoadScene(nombreEscenaDestino);
    }
}