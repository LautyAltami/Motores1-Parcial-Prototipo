using UnityEngine;
using UnityEngine.UI;

public class SanityManager : MonoBehaviour
{
    [Header("Estado de Sanidad")]
    public float currentSanity = 100f;
    public bool canDieFromSanity = true;

    [Header("Variables del Entorno")]
    public bool isHidden = false;

    [Header("Tasas de Cambio")]
    public float chasingDrainRate = 5f;
    public float lockerRecoveryRate = 15f;
    public float monsterMultiplier = 5f;

    [Header("UI Barra de Cordura")]
    public Image sanityBar;

    [Header("Audio Ambiente (loops simples)")]
    public AudioSource sourceExploracion;
    public AudioSource sourceLocura;
    public float velocidadFadeAmbiente = 2f;

    [Header("Sistema de Audio Dinamico (cordura baja)")]
    public AudioSource sourceRespiracion;
    public AudioSource sourceSusurros;

    // --- NUEVAS VARIABLES DE LATIDOS ---
    public AudioSource sourceLatidos;
    [Range(0f, 100f)]
    public float inicioLatidos = 70f; // Empiezan a sonar antes que los susurros para ir avisando
    public float minPitchLatidos = 1.0f; // Velocidad normal
    public float maxPitchLatidos = 1.6f; // Taquicardia al 0% de cordura
    // -----------------------------------

    [Range(0f, 100f)]
    public float inicioSusurros = 50f;
    public float velocidadFade = 2f;

    private Transform monster;
    private ShadowAI monsterAI;

    private void OnEnable()
    {
        GameManager.OnMonstruoSpawnea += MonstruoAparecio;
    }

    private void OnDisable()
    {
        GameManager.OnMonstruoSpawnea -= MonstruoAparecio;
    }

    private void MonstruoAparecio(GameObject monstruo)
    {
        if (monstruo != null)
        {
            monster = monstruo.transform;
            monsterAI = monstruo.GetComponent<ShadowAI>();
        }
    }

    void Start()
    {
        if (sanityBar != null) sanityBar.gameObject.SetActive(false);

        if (sourceRespiracion != null) { sourceRespiracion.loop = true; sourceRespiracion.volume = 0f; sourceRespiracion.Play(); }
        if (sourceSusurros != null) { sourceSusurros.loop = true; sourceSusurros.volume = 0f; sourceSusurros.Play(); }

        // --- INICIAMOS EL LATIDO EN SILENCIO Y VELOCIDAD NORMAL ---
        if (sourceLatidos != null) { sourceLatidos.loop = true; sourceLatidos.volume = 0f; sourceLatidos.pitch = minPitchLatidos; sourceLatidos.Play(); }

        if (sourceExploracion != null) { sourceExploracion.loop = true; sourceExploracion.volume = 1f; sourceExploracion.Play(); }
        if (sourceLocura != null) { sourceLocura.loop = true; sourceLocura.volume = 0f; sourceLocura.Play(); }
    }

    void Update()
    {
        bool monstruoPersiguiendo = monsterAI != null && monsterAI.IsChasing;

        if (isHidden)
        {
            currentSanity += lockerRecoveryRate * Time.deltaTime;
        }
        else if (monstruoPersiguiendo)
        {
            float currentDrain = chasingDrainRate;

            if (monster != null)
            {
                float distance = Vector3.Distance(transform.position, monster.position);
                if (distance < 10f)
                {
                    currentDrain += monsterMultiplier * (10f / Mathf.Max(distance, 1f));
                }
            }

            currentSanity -= currentDrain * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, 100f);

        ActualizarBarraUI(monstruoPersiguiendo);
        ActualizarAudioAmbiente(monstruoPersiguiendo);
        ActualizarAudioPsicologico();

        if (currentSanity <= 0 && canDieFromSanity)
        {
            DieFromInsanity();
        }
    }

    void ActualizarBarraUI(bool monstruoPersiguiendo)
    {
        if (sanityBar == null) return;

        sanityBar.fillAmount = currentSanity / 100f;

        if (monstruoPersiguiendo && !sanityBar.gameObject.activeSelf)
        {
            sanityBar.gameObject.SetActive(true);
        }
        else if (currentSanity >= 100f && sanityBar.gameObject.activeSelf)
        {
            sanityBar.gameObject.SetActive(false);
        }
    }

    void ActualizarAudioAmbiente(bool monstruoPersiguiendo)
    {
        float volumenExploracionObjetivo = monstruoPersiguiendo ? 0f : 1f;
        float volumenLocuraObjetivo = monstruoPersiguiendo ? 1f : 0f;

        if (sourceExploracion != null)
            sourceExploracion.volume = Mathf.Lerp(sourceExploracion.volume, volumenExploracionObjetivo, Time.deltaTime * velocidadFadeAmbiente);

        if (sourceLocura != null)
            sourceLocura.volume = Mathf.Lerp(sourceLocura.volume, volumenLocuraObjetivo, Time.deltaTime * velocidadFadeAmbiente);
    }

    void ActualizarAudioPsicologico()
    {
        // 1. SUSURROS
        float volumenSusurrosObjetivo = 0f;
        if (currentSanity <= inicioSusurros)
        {
            volumenSusurrosObjetivo = Mathf.InverseLerp(inicioSusurros, 0f, currentSanity);
        }

        if (sourceSusurros != null)
        {
            sourceSusurros.volume = Mathf.Lerp(sourceSusurros.volume, volumenSusurrosObjetivo, Time.deltaTime * velocidadFade);
        }

        // --- 2. LATIDOS (VOLUMEN Y PITCH DINÁMICO) ---
        float volumenLatidosObjetivo = 0f;
        float pitchLatidosObjetivo = minPitchLatidos;

        if (currentSanity <= inicioLatidos)
        {
            // Calcula un factor de 0 a 1 dependiendo de qué tan baja esté la cordura
            float factorLatido = Mathf.InverseLerp(inicioLatidos, 0f, currentSanity);

            volumenLatidosObjetivo = factorLatido;
            // Acelera el audio interpolando entre el minPitch y el maxPitch
            pitchLatidosObjetivo = Mathf.Lerp(minPitchLatidos, maxPitchLatidos, factorLatido);
        }

        if (sourceLatidos != null)
        {
            sourceLatidos.volume = Mathf.Lerp(sourceLatidos.volume, volumenLatidosObjetivo, Time.deltaTime * velocidadFade);
            sourceLatidos.pitch = Mathf.Lerp(sourceLatidos.pitch, pitchLatidosObjetivo, Time.deltaTime * velocidadFade);
        }
        // ----------------------------------------------

        // 3. RESPIRACIÓN (Cuando estás escondido)
        if (isHidden)
        {
            float volumenRespiracionObjetivo = Mathf.Clamp01((100f - currentSanity) / 100f);

            if (sourceRespiracion != null)
            {
                sourceRespiracion.volume = Mathf.Lerp(sourceRespiracion.volume, volumenRespiracionObjetivo, Time.deltaTime * velocidadFade);
            }
        }
        else
        {
            if (sourceRespiracion != null)
            {
                sourceRespiracion.volume = 0f;
            }
        }
    }

    void DieFromInsanity()
    {
        this.enabled = false;
    }
}