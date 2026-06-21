using UnityEngine;

// Las luces (READY/OK/ERROR) ya vienen con su color de Emission configurado
// en Blender/el material. Este script solo prende y apaga los GameObjects,
// y le avisa a la puerta que se abra cuando la tarjeta es correcta.
[RequireComponent(typeof(AudioSource))]
public class CardReader : ObjetoInteractivoBase
{
    [Header("Configuracion")]
    public string requiredCardId = "TarjetaRoja"; // Mismo ID que usa la puerta en requiredKeyId
    public string uiText = "Lector de Tarjeta";

    [Header("Puerta a abrir cuando la tarjeta es correcta")]
    public DoorScriptLocked puerta; // Arrastrar aca la puerta que controla este lector

    [Header("Luces del lector (GameObjects, ya con su Emission seteado)")]
    public GameObject luzOk;      // La esfera "OK" (verde)
    public GameObject luzError;   // La esfera "ERROR" (roja)
    public float duracionLuz = 1.5f; // Cuanto tiempo queda prendida antes de apagarse

    [Header("Audio")]
    public AudioClip sonidoOk;
    public AudioClip sonidoError;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ApagarLuces();
    }

    public override string ObtenerMensaje()
    {
        return uiText;
    }

    public override void Interact(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Error: El jugador no tiene el script PlayerInventory.");
            return;
        }

        if (inventory.HasKey(requiredCardId))
        {
            if (luzOk != null) luzOk.SetActive(true);
            if (sonidoOk != null) audioSource.PlayOneShot(sonidoOk);

            // Le decimos a la puerta que se abra, reusando su propia logica de Interact.
            // La puerta ya tiene "isLocked" y va a chequear HasKey igual, pero como
            // ya confirmamos que tiene la tarjeta, esto la destraba y abre directo.
            if (puerta != null)
                puerta.Interact(player);
        }
        else
        {
            if (luzError != null) luzError.SetActive(true);
            if (sonidoError != null) audioSource.PlayOneShot(sonidoError);
        }

        Invoke(nameof(ApagarLuces), duracionLuz);
    }

    void ApagarLuces()
    {
        if (luzOk != null) luzOk.SetActive(false);
        if (luzError != null) luzError.SetActive(false);
    }
}