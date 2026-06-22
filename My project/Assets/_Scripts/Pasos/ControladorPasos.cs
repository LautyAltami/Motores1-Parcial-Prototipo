using UnityEngine;

public class ControladorPasos : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource sourcePasos;
    public AudioClip[] audiosPasos; // Acá vas a arrastrar tus 3 audios sueltos

    [Header("Configuración Caminar")]
    public float tiempoCaminar = 0.6f; // Segundos entre cada paso
    public float volumenCaminar = 0.5f;
    public float pitchCaminar = 1.0f;

    [Header("Configuración Correr (El Truco Indie)")]
    public float tiempoCorrer = 0.35f; // Más rápido (menos tiempo entre pasos)
    public float volumenCorrer = 1.0f; // Más fuerte (impacto)
    public float pitchCorrer = 0.9f;   // Más grave (sensación de peso)

    private float timerPasos = 0f;

    void Update()
    {
        // 1. Detectamos si el jugador se está moviendo (reemplazá con tus teclas si son otras)
        bool seMueve = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // 2. Detectamos si está presionando Shift para correr
        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift);

        if (seMueve)
        {
            // El temporizador va bajando
            timerPasos -= Time.deltaTime;

            // Cuando llega a 0, es hora de reproducir un paso
            if (timerPasos <= 0f)
            {
                ReproducirPaso(estaCorriendo);

                // Reiniciamos el temporizador dependiendo de si corre o camina
                timerPasos = estaCorriendo ? tiempoCorrer : tiempoCaminar;
            }
        }
        else
        {
            // Si nos quedamos quietos, el timer se resetea para que el primer paso suene instantáneo al arrancar
            timerPasos = 0f;
        }
    }

    void ReproducirPaso(bool corriendo)
    {
        // Seguridad por si te olvidaste de poner los audios
        if (audiosPasos.Length == 0) return;

        // Elegimos uno de los 3 pasos AL AZAR
        int indiceAleatorio = Random.Range(0, audiosPasos.Length);

        // APLICAMOS EL TRUCO INDIE
        sourcePasos.volume = corriendo ? volumenCorrer : volumenCaminar;

        // Le sumamos un pequeño random al Pitch para que NUNCA suene exactamente igual (rompe la repetición robótica)
        float pitchBase = corriendo ? pitchCorrer : pitchCaminar;
        sourcePasos.pitch = pitchBase + Random.Range(-0.05f, 0.05f);

        // Reproducimos el audio sin cortar el anterior (PlayOneShot es clave para efectos de sonido)
        sourcePasos.PlayOneShot(audiosPasos[indiceAleatorio]);
    }
}