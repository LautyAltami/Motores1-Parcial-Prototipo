using UnityEngine;

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
    public string requiredCardId = "TarjetaRoja";
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

        // DEBUG TEMPORAL: confirma que este script arranco bien
        Debug.Log("[DoorScriptCardLocked] Start() ejecutado. isLocked=" + isLocked);
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

    public void DesbloquearDesdeTarjetero()
    {
        // DEBUG TEMPORAL: esto TIENE que aparecer en consola al tocar el tarjetero con OK
        Debug.Log("[DoorScriptCardLocked] DesbloquearDesdeTarjetero() fue llamado. isLocked antes=" + isLocked);

        if (!isLocked)
        {
            Debug.Log("[DoorScriptCardLocked] Ya estaba destrabada, no hago nada.");
            return;
        }

        isLocked = false;

        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);

        isOpen = true;
        targetRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;

        Debug.Log("[DoorScriptCardLocked] Puerta destrabada y abierta. isLocked ahora=" + isLocked);
    }
}