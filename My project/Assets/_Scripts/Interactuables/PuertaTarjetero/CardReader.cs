using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CardReader : ObjetoInteractivoBase
{
    [Header("Configuracion")]
    public string requiredCardId = "TarjetaRoja";
    public string uiText = "Lector de Tarjeta";

    [Header("Puerta a abrir cuando la tarjeta es correcta")]
    public DoorScriptCardLocked puerta;

    [Header("Luces del lector (GameObjects, ya con su Emission seteado)")]
    public GameObject luzOk;
    public GameObject luzError;
    public float duracionLuz = 1.5f;

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
        // DEBUG TEMPORAL
        Debug.Log("[CardReader] Interact() llamado.");

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Error: El jugador no tiene el script PlayerInventory.");
            return;
        }

        bool tieneTarjeta = inventory.HasKey(requiredCardId);
        Debug.Log("[CardReader] HasKey(" + requiredCardId + ") = " + tieneTarjeta);

        if (tieneTarjeta)
        {
            if (luzOk != null) luzOk.SetActive(true);
            if (sonidoOk != null) audioSource.PlayOneShot(sonidoOk);

            Debug.Log("[CardReader] Intentando llamar a puerta.DesbloquearDesdeTarjetero(). Puerta es null? " + (puerta == null));

            if (puerta != null)
                puerta.DesbloquearDesdeTarjetero();
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