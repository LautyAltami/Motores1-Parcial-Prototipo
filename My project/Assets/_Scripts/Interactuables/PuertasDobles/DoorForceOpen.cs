using UnityEngine;

// Poner este script en CADA hoja de la puerta doble (son 2 objetos distintos)//

public class DoorForceOpen : MonoBehaviour
{
    [Header("Locker que activa ESTA puerta")]
    public Transform lockerQueLaActiva; // Arrastrar aca el locker de Fase 2, o el de Administracion, segun corresponda

    [Header("Configuracion de Rotacion")]
    public Transform pivot;              // Si esta vacio, usa este mismo transform
    public Vector3 anguloAbierto = new Vector3(0f, 100f, 0f); // Rotacion local final (probar en editor)
    public float velocidadApertura = 4f; // Que tan rapido gira al forzar la apertura

    [Header("Audio")]
    public AudioClip sonidoGolpazo; // El audio del portazo / rotura

    [Header("Collider de paso (opcional)")]
    public Collider colliderDeBloqueo; // Si hay un collider que bloqueaba el paso, se desactiva al abrir

    private AudioSource audioSource;
    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;
    public bool forzandoApertura = false;
    private bool yaSeAbrio = false;

    void Awake()
    {
        if (pivot == null) pivot = transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        rotacionCerrada = pivot.localRotation;
        rotacionAbierta = rotacionCerrada * Quaternion.Euler(anguloAbierto);
    }

    void OnEnable()
    {
        ShadowAI.OnDespawn += OnMonstruoDespawneo;
    }

    void OnDisable()
    {
        ShadowAI.OnDespawn -= OnMonstruoDespawneo;
    }

    // Se llama SIEMPRE que el monstruo despawnea en CUALQUIER locker del nivel.
    // Por eso primero chequeamos si el locker que lo activo es el nuestro.
    private void OnMonstruoDespawneo(Transform lockerQueDisparo)
    {
        if (lockerQueDisparo != lockerQueLaActiva) return; // No es nuestro locker, ignoramos
        AbrirForzado();
    }

    void Update()
    {
        if (forzandoApertura)
        {
            pivot.localRotation = Quaternion.Lerp(pivot.localRotation, rotacionAbierta, Time.deltaTime * velocidadApertura);
        }
    }

    public void AbrirForzado()
    {
        if (yaSeAbrio) return; // Solo se abre una vez
        yaSeAbrio = true;

        if (colliderDeBloqueo != null)
            colliderDeBloqueo.enabled = false;

        if (sonidoGolpazo != null)
            audioSource.PlayOneShot(sonidoGolpazo);

        forzandoApertura = true;
    }

    // Util para probar el angulo en el editor sin correr el juego:
    // click derecho en el componente -> "Probar Apertura (Editor)"
    [ContextMenu("Probar Apertura (Editor)")]
    void ProbarAperturaEditor()
    {
        if (pivot == null) pivot = transform;
        pivot.localRotation = pivot.localRotation * Quaternion.Euler(anguloAbierto);
    }
}