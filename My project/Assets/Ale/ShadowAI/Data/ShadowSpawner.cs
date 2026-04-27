using UnityEngine;

public class ShadowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private Transform spawnPoint;

    public void SpawnShadow()
    {
        Instantiate(
            shadowPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }
}