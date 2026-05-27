using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    public int idDeEsteSpawn;

    [Header("Condiciones")]
    public bool requiereLlave = false;
    public string keyId = "HospitalKey";

    public bool requiereLinterna = false;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        Flashlight flashlight = other.GetComponentInChildren<Flashlight>();

        // CHECK DE CONDICIONES
        if (requiereLlave && (inventory == null || !inventory.HasKey(keyId)))
            return;

        if (requiereLinterna && (flashlight == null || !flashlight.estaEquipada))
            return;

        // TODO OK → SPAWN
        activado = true;

        GameManager.DispararSpawn(idDeEsteSpawn);

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }
}