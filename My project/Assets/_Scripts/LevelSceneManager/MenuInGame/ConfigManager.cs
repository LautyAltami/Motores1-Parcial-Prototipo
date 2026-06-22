using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ConfiguracionManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderSensibilidad;
    public Slider sliderVolumen;

    [Header("Sensibilidad")]
    public float sensibilidadMin = 100f;
    public float sensibilidadMax = 1000f;
    public float sensibilidadDefault = 500f;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public string nombreParametroVolumen = "MasterVolume";
    public float volumenDefault = 0.8f; // El jugador arranca con el volumen al 80%

    [Header("Referencia al jugador")]
    public MouseLook scriptDeMirada;

    void Start()
    {
        BuscarAlJugador();
        CargarValoresGuardados();
    }

    void BuscarAlJugador()
    {
        if (scriptDeMirada == null)
        {
            scriptDeMirada = FindObjectOfType<MouseLook>();
        }
    }

    void CargarValoresGuardados()
    {
        float sensGuardada = PlayerPrefs.GetFloat("Sensibilidad", sensibilidadDefault);
        float volGuardado = PlayerPrefs.GetFloat("Volumen", volumenDefault);

        // Reseteo de seguridad por si había datos corruptos guardados
        if (sensGuardada < sensibilidadMin || sensGuardada > sensibilidadMax)
        {
            sensGuardada = sensibilidadDefault;
        }

        if (sliderSensibilidad != null)
        {
            sliderSensibilidad.minValue = sensibilidadMin;
            sliderSensibilidad.maxValue = sensibilidadMax;
            sliderSensibilidad.value = sensGuardada;
        }

        if (sliderVolumen != null)
        {
            // OBLIGAMOS al slider visual a ser un porcentaje (0.0001 a 1)
            // Usamos 0.0001 en vez de 0 absoluto para que la matemática no colapse
            sliderVolumen.minValue = 0.0001f;
            sliderVolumen.maxValue = 1f;
            sliderVolumen.value = volGuardado;
        }

        AplicarSensibilidad(sensGuardada);
        AplicarVolumen(volGuardado);
    }

    public void CambiarSensibilidad(float nuevoValor)
    {
        AplicarSensibilidad(nuevoValor);
        PlayerPrefs.SetFloat("Sensibilidad", nuevoValor);
        PlayerPrefs.Save();
    }

    public void CambiarVolumen(float nuevoValor)
    {
        AplicarVolumen(nuevoValor);
        PlayerPrefs.SetFloat("Volumen", nuevoValor);
        PlayerPrefs.Save();
    }

    void AplicarSensibilidad(float valor)
    {
        if (scriptDeMirada == null)
        {
            BuscarAlJugador();
        }

        if (scriptDeMirada != null)
        {
            scriptDeMirada.mouseSensitivity = valor;
        }
    }

    void AplicarVolumen(float valor)
    {
        if (audioMixer != null)
        {
            // Traduce el porcentaje del Slider (ej: 0.5) a Decibeles para el Mixer de Unity
            float db = Mathf.Log10(valor) * 20f;
            audioMixer.SetFloat(nombreParametroVolumen, db);
        }
    }
}