using UnityEngine;

// Variante de DoorScriptLocked pensada para puertas que se abren con TARJETA
// a traves del CardReader, no tocando la puerta directamente.
//
// Diferencia clave con DoorScriptLocked: esta puerta NUNCA se destraba sola
// al tocarla, sin importar si el jugador tiene la tarjeta o no. Solo se
// destraba cuando el CardReader llama a DesbloquearDesdeTarjetero().
public class DoorScriptCardLocked : ObjetoInteractivoBase
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
    public string requiredCardId = "TarjetaRoja"; // Solo informativo para el cartel, no se chequea aca
    public AudioSource audioSource;
    public AudioClip lockedSound;
    public AudioClip unlockSound;

    [Header("Texto del cartel cuando esta trabada")]
    public string textoTrabada = "Necesitas la Tarjeta Roja...";

    private bool mostrarCartelTrabada = false;
    private float timerCartel = 0f;

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
        pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, Time.deltaTime * openVelocity);

        if (mostrarCartelTrabada)
        {
            timerCartel -= Time.deltaTime;
            if (timerCartel <= 0) mostrarCartelTrabada = false;
        }
    }

    public override string ObtenerMensaje()
    {
        if (mostrarCartelTrabada) return textoTrabada;
        if (isLocked) return "";
        return isOpen ? "Cerrar puerta" : "Abrir puerta";
    }

    // Al tocar la puerta directamente: si esta trabada, SIEMPRE muestra el cartel,
    // nunca la destraba, sin importar si el jugador tiene la tarjeta o no.
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

        isOpen = !isOpen;
        targetRotation = isOpen ? Quaternion.Euler(0, openAngle, 0) * closedRotation : closedRotation;
    }

    // Esta es la UNICA forma de destrabar y abrir la puerta: llamada por el
    // CardReader cuando confirma que el jugador tiene la tarjeta correcta.
    public void DesbloquearDesdeTarjetero()
    {
        if (!isLocked) return; // Ya estaba destrabada, no hacemos nada

        isLocked = false;

        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);

        // La abrimos directamente
        isOpen = true;
        targetRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }
}