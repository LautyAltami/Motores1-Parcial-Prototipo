using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    [Header("Spawn")]
    public SpawnPoint spawnPoint;

    [Header("Condiciones")]
    public bool requiereLlave = false;
    public string keyId = "HospitalKey";

    public bool requiereLinterna = false;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        Flashlight flashlight = other.GetComponentInChildren<Flashlight>();

        // Verificar llave
        if (requiereLlave &&
            (inventory == null || !inventory.HasKey(keyId)))
        {
            return;
        }

        // Verificar linterna
        if (requiereLinterna &&
            (flashlight == null || !flashlight.estaEquipada))
        {
            return;
        }

        activado = true;

        GameManager.DispararSpawn(spawnPoint);

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }
}