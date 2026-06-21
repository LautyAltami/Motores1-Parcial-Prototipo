using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Te asegura que la llave tenga un parlante
public class InteractableKey : ObjetoInteractivoBase
{
    public string keyId = "HospitalKey";
    public AudioClip grabSound;
    public string uiText = "LLave";

    private AudioSource audioSource;

    void Start()
    {
        // Guardamos la referencia al Audio Source al arrancar
        audioSource = GetComponent<AudioSource>();
    }

    public override string ObtenerMensaje()
    {
        return uiText;
    }

    public override void Interact(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddKey(keyId);

            if (grabSound != null)
            {
                // Reproducimos el sonido a través de NUESTRO Audio Source (que va al Mixer)
                audioSource.PlayOneShot(grabSound);
            }

            // TRUCO DE AUDIO: Ocultamos la llave y desactivamos el collider
            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = false;

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;

            // Destruimos el objeto pero con un retraso (la duración exacta del audio)
            float duracionSonido = grabSound != null ? grabSound.length : 0.1f;
            Destroy(gameObject, duracionSonido);
        }
        else
        {
            Debug.LogWarning("Error: El jugador no tiene el script PlayerInventory.");
        }
    }
}