using UnityEngine;
using UnityEngine.UI;

public class SanityManager : MonoBehaviour
{
    [Header("Estado de Sanidad")]
    public float currentSanity = 100f;
    public bool canDieFromSanity = true;

    [Header("Variables del Entorno")]
    public bool isHidden = false;
    public bool hasLight = false;

    [Header("Tasas de Cambio")]
    public float darknessDrainRate = 3f; // Cuánto drena la oscuridad por segundo
    public float lockerRecoveryRate = 15f; // Cuánto recupera el locker por segundo (ajustable para que sea más rápido o más lento)
    public float monsterMultiplier = 5f; // Cuánto aumenta el drenaje por cercanía al monstruo (ajustable para que sea un aumento suave o muy agresivo)

    [Header("UI Barra de Cordura")]
    public Image sanityBar;

    [Header("Sistema de Audio Dinámico")]
    public AudioSource sourceRespiracion;
    public AudioSource sourceSusurros;
    [Range(0f, 100f)]
    public float inicioSusurros = 50f; // A partir de qué nivel de cordura arrancan los susurros
    public float velocidadFade = 2f;   // Qué tan suave es el cambio de volumen

    private Transform monster;

    // Nos suscribimos al evento de spawn del monstruo para saber cuándo aparece y activar la barra de cordura
    private void OnEnable()
    {
        GameManager.OnMonstruoSpawnea += MonstruoAparecio;
    }

   // Nos desuscribimos del evento de spawn del monstruo para evitar errores al destruir este objeto
   private void OnDisable()
    {
        
        GameManager.OnMonstruoSpawnea -= MonstruoAparecio;
    }

    private void MonstruoAparecio(int idSpawn) // Ajustado para recibir el ID del evento que armamos
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Enemy");
        if (obj != null) monster = obj.transform;

        if (sanityBar != null) sanityBar.gameObject.SetActive(true);
    }

    void Start()
    {
        if (sanityBar != null) sanityBar.gameObject.SetActive(false);

        // Nos aseguramos que los audios estén loopeados pero muteados al inicio
        if (sourceRespiracion != null) { sourceRespiracion.loop = true; sourceRespiracion.volume = 0f; sourceRespiracion.Play(); }
        if (sourceSusurros != null) { sourceSusurros.loop = true; sourceSusurros.volume = 0f; sourceSusurros.Play(); }
    }

    void Update()
    {
        // --- LÓGICA DE CORDURA ---
        if (isHidden)
        {
            currentSanity += lockerRecoveryRate * Time.deltaTime;
        }
        // Si no está escondido, la cordura se drena por la oscuridad y la cercanía del monstruo
        else
        {
            float currentDrain = 0f;
           
            // Si no hay luz, drena cordura. Si hay luz, no drena por oscuridad.
            if (!hasLight) currentDrain += darknessDrainRate;
            
            // La cercanía del monstruo aumenta el drenaje, pero solo si el monstruo existe
            if (monster != null)
            {
                float distance = Vector3.Distance(transform.position, monster.position);
                // Si el monstruo está a menos de 10 unidades, empieza a drenar más rápido. A medida que se acerca, el drenaje aumenta exponencialmente.
                if (distance < 10f)
                {
                    currentDrain += monsterMultiplier * (10f / Mathf.Max(distance, 1f));
                }
            }
            currentSanity -= currentDrain * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, 100f);
        sanityBar.fillAmount = currentSanity / 100f;
        // --- LLAMADA AL MÓDULO DE AUDIO ---
        ActualizarAudioPsicologico();

        if (currentSanity <= 0 && canDieFromSanity)
        {
            DieFromInsanity();
        }
    }

    void ActualizarAudioPsicologico()
    {
        // 1. Lógica de Susurros (Para el juego en general, cuando estás bajo de cordura)
        float volumenSusurrosObjetivo = 0f;
        if (currentSanity <= inicioSusurros)
        {
            volumenSusurrosObjetivo = Mathf.InverseLerp(inicioSusurros, 0f, currentSanity);
        }

        if (sourceSusurros != null)
        {
            sourceSusurros.volume = Mathf.Lerp(sourceSusurros.volume, volumenSusurrosObjetivo, Time.deltaTime * velocidadFade);
        }

        // 2. Lógica de Respiración (EXCLUSIVA DEL CASILLERO)
        if (isHidden)
        {
            // Adentro del casillero: El volumen baja suavemente a medida que te calmás
            float volumenRespiracionObjetivo = Mathf.Clamp01((100f - currentSanity) / 100f);

            if (sourceRespiracion != null)
            {
                sourceRespiracion.volume = Mathf.Lerp(sourceRespiracion.volume, volumenRespiracionObjetivo, Time.deltaTime * velocidadFade);
            }
        }
        else
        {
            // Afuera del casillero: CORTE SECO INSTANTÁNEO
            if (sourceRespiracion != null)
            {
                sourceRespiracion.volume = 0f;
            }
        }
    }

    void DieFromInsanity()
    {
        // GameManager.EjecutarGameOver();
        this.enabled = false;
    }
}