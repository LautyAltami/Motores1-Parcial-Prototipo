using UnityEngine;

public class DoorOpen : ObjetoInteractivoBase
{
    [Header("Configuracion de Movimiento")]
    public Transform pivot;
    public float openAngle = 90f;
    public float openVelocity = 2f;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private bool isOpen = false;

    [Header("Logica de Cerradura")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    protected override void Start()
    {
        base.Start(); // faltaba esto
        if (pivot == null) pivot = transform;
        closedRotation = pivot.rotation;
        targetRotation = closedRotation;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, Time.deltaTime * openVelocity);
    }

    public override void Interact(GameObject player)
    {
        isOpen = !isOpen;
        targetRotation = isOpen
            ? Quaternion.Euler(0, openAngle, 0) * closedRotation
            : closedRotation;

        if (audioSource != null)
        {
            AudioClip clip = isOpen ? openSound : closeSound;
            if (clip != null) audioSource.PlayOneShot(clip);
        }
    }

    public override string ObtenerMensaje()
    {
        return isOpen ? "Cerrar puerta" : "Abrir puerta";
    }
}