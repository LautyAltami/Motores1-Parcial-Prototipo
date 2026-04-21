using UnityEngine;

// Heredamos de la base para ganar el sistema de Outline automáticamente
public class DoorScript : ObjetoInteractivoBase
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
    public AudioSource audioSource;
    public AudioClip lockedSound;

    private bool mostrarCartelTrabada = false;
    private float timerCartel = 0f;

    // --- IMPORTANTE: Usamos override y llamamos a la base ---
    protected override void Start()
    {
        base.Start(); // Esto busca el Outline en el objeto

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

    // Implementamos el mensaje requerido por la base
    public override string ObtenerMensaje()
    {
        if (mostrarCartelTrabada) return "Necesitas una llave...";

        if (isLocked) return ""; // No mostramos nada hasta que interactúe

        return isOpen ? "Cerrar puerta" : "Abrir puerta";
    }

    // Implementamos la interacción requerida por la base
    public override void Interact(GameObject player)
    {
        if (isLocked)
        {
            if (audioSource != null && lockedSound != null)
                audioSource.PlayOneShot(lockedSound);

            mostrarCartelTrabada = true;
            timerCartel = 2f;
            return;
        }

        // Lógica de apertura/cierre
        isOpen = !isOpen;
        targetRotation = isOpen ? Quaternion.Euler(0, openAngle, 0) * closedRotation : closedRotation;
    }
}