using UnityEngine;

// Heredamos de la base para ganar el sistema de Outline automáticamente
public class DoorScriptLocked: ObjetoInteractivoBase
{
    [Header("Configuracion de Movimiento")]
    public Transform pivot;
    public float openAngle = 90f;
    public float openVelocity = 2f;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private bool isOpen = false;

    [Header("Logica de Cerradura")]
    public bool isLocked = true;
    public string requiredKeyId = "HospitalKey"; // <-- Agregamos el ID de la llave
    public AudioSource audioSource;
    public AudioClip lockedSound;
    public AudioClip unlockSound; // <-- Opcional: Sonido de llave girando

    private bool mostrarCartelTrabada = false;
    private float timerCartel = 0f;

    // --- IMPORTANTE: Usamos override y llamamos a la base ---
    protected override void Start()
    {
        base.Start();

        if (pivot == null) pivot = transform;
        closedRotation = pivot.rotation;
        targetRotation = closedRotation;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Movimiento suave del pivot
        pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, Time.deltaTime * openVelocity);

        // Timer para el mensaje de "Necesitas llave"
        if (mostrarCartelTrabada)
        {
            timerCartel -= Time.deltaTime;
            if (timerCartel <= 0) mostrarCartelTrabada = false;
        }
    }

    public override string ObtenerMensaje()
    {
        if (mostrarCartelTrabada) return "Necesitas una llave...";

        // Si está trabada, no mostramos nada (o podes poner "Abrir Puerta" si querés que prueben)
        if (isLocked) return "";

        return isOpen ? "Cerrar puerta" : "Abrir puerta";
    }

    public override void Interact(GameObject player)
    {
        // 1. Si la puerta está trabada, intentamos destrabarla
        if (isLocked)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

            // ¿Tiene el jugador la llave correcta?
            if (inventory != null && inventory.HasKey(requiredKeyId))
            {
                // ¡Éxito! Destrabamos la puerta
                isLocked = false;
                Debug.Log("Puerta destrabada usando la llave: " + requiredKeyId);

                if (audioSource != null && unlockSound != null)
                    audioSource.PlayOneShot(unlockSound);
            }
            else
            {
                // No tiene la llave. Hacemos sonar la manija y mostramos el cartel
                if (audioSource != null && lockedSound != null)
                    audioSource.PlayOneShot(lockedSound);

                mostrarCartelTrabada = true;
                timerCartel = 2f;
                return; // Cortamos la función acá para que no se abra
            }
        }

        // 2. Si llegamos acá, la puerta no está trabada (o la acabamos de destrabar)
        // Lógica de apertura/cierre
        isOpen = !isOpen;
        targetRotation = isOpen ? Quaternion.Euler(0, openAngle, 0) * closedRotation : closedRotation;
    }
}