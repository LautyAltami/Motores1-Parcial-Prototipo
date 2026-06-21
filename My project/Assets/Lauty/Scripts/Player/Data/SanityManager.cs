using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

    [Header("UI Barra de Cordura")]
    public Image sanityBar;

    [Header("Sistema de Audio Dinámico")]
    public AudioSource sourceRespiracion;
    public AudioSource sourceSusurros;
    [Range(0f, 100f)]
    public float inicioSusurros = 50f;
    public float velocidadFade = 2f;

    [Header("Snapshots del Mixer")]
    public AudioMixerSnapshot snapshotExploracion;
    public AudioMixerSnapshot snapshotLocura;
    public float tiempoTransicionSnapshot = 3f;
    private bool enLocura = false;

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

    private void MonstruoAparecio(int idSpawn)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Enemy");
        if (obj != null)
        {
            monster = obj.transform;
            monsterAI = obj.GetComponent<ShadowAI>();
        }
    }

    void Start()
    {
        // La barra arranca oculta, solo aparece cuando el monstruo persigue
        if (sanityBar != null) sanityBar.gameObject.SetActive(false);

        if (sourceRespiracion != null) { sourceRespiracion.loop = true; sourceRespiracion.volume = 0f; sourceRespiracion.Play(); }
        if (sourceSusurros != null) { sourceSusurros.loop = true; sourceSusurros.volume = 0f; sourceSusurros.Play(); }

        if (snapshotExploracion != null) snapshotExploracion.TransitionTo(0f);
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
            currentSanity -= chasingDrainRate * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, 100f);

        ActualizarBarraUI(monstruoPersiguiendo);
        ActualizarAudioPsicologico();
        ActualizarSnapshot(monstruoPersiguiendo);

        if (currentSanity <= 0 && canDieFromSanity)
        {
            DieFromInsanity();
        }
    }

    void ActualizarBarraUI(bool monstruoPersiguiendo)
    {
        if (sanityBar == null) return;

        sanityBar.fillAmount = currentSanity / 100f;

        // Se muestra mientras persigue. Se oculta solo cuando la cordura volvió al 100%.
        if (monstruoPersiguiendo && !sanityBar.gameObject.activeSelf)
        {
            sanityBar.gameObject.SetActive(true);
        }
        else if (currentSanity >= 100f && sanityBar.gameObject.activeSelf)
        {
            sanityBar.gameObject.SetActive(false);
        }
    }

    void ActualizarSnapshot(bool monstruoPersiguiendo)
    {
        if (monstruoPersiguiendo && !enLocura)
        {
            snapshotLocura.TransitionTo(tiempoTransicionSnapshot);
            enLocura = true;
        }
        else if (!monstruoPersiguiendo && enLocura)
        {
            snapshotExploracion.TransitionTo(tiempoTransicionSnapshot);
            enLocura = false;
        }
    }

    void ActualizarAudioPsicologico()
    {
        float volumenSusurrosObjetivo = 0f;
        if (currentSanity <= inicioSusurros)
        {
            volumenSusurrosObjetivo = Mathf.InverseLerp(inicioSusurros, 0f, currentSanity);
        }

        if (sourceSusurros != null)
        {
            sourceSusurros.volume = Mathf.Lerp(sourceSusurros.volume, volumenSusurrosObjetivo, Time.deltaTime * velocidadFade);
        }

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