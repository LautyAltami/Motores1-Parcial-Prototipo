using UnityEngine;

// Script universal para items que se guardan en el inventario como "key":
// llaves normales (HospitalKey), tarjetas (TarjetaRoja, TarjetaVerde), etc.
// Todos usan el mismo sistema de PlayerInventory.AddKey(id) / HasKey(id),
// la unica diferencia practica es el texto que se muestra en pantalla.
[RequireComponent(typeof(AudioSource))]
public class InteractableKey : ObjetoInteractivoBase
{
    public string keyId = "HospitalKey"; // Puede ser "HospitalKey", "TarjetaRoja", "TarjetaVerde", etc.
    public AudioClip grabSound;
    public string uiText = "Llave"; // Cambiar a "Tarjeta Roja" en las tarjetas, por ejemplo

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
                // Reproducimos el sonido a traves de NUESTRO Audio Source (que va al Mixer)
                audioSource.PlayOneShot(grabSound);
            }

            // TRUCO DE AUDIO: Ocultamos el objeto y desactivamos el collider
            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = false;

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;

            // Destruimos el objeto pero con un retraso (la duracion exacta del audio)
            float duracionSonido = grabSound != null ? grabSound.length : 0.1f;
            Destroy(gameObject, duracionSonido);
        }
        else
        {
            Debug.LogWarning("Error: El jugador no tiene el script PlayerInventory.");
        }
    }
}