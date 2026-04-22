using UnityEngine;

public class InteractableKey : ObjetoInteractivoBase
{
    public string keyId = "HospitalKey";
    public AudioClip grabSound;

    // Mantenemos el nombre en español para no romper tu ObjetoInteractivoBase
    public override string ObtenerMensaje()
    {
        return "Grab Key"; // El texto de la UI sí lo pasamos a inglés
    }

    public override void Interact(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddKey(keyId);

            if (grabSound != null)
                AudioSource.PlayClipAtPoint(grabSound, transform.position);

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Error: El jugador no tiene el script PlayerInventory.");
        }
    }
}