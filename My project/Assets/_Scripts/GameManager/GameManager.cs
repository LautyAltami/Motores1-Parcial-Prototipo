using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Configuración del Monstruo")]
    public GameObject monstruoPrefab;

    [Header("Configuración de Audio")]
    public AudioClip ScreamerSound;
    public float VolumeScreamer = 1.0f;
    public AudioSource musicaTension;

    // Evento cuando aparece un monstruo
    public static event Action<GameObject> OnMonstruoSpawnea;

    // Eventos de victoria y derrota
    public static event Action OnGameWin;
    public static event Action OnGameLose;

    public static void DispararSpawn(SpawnPoint spawn)
    {
        if (spawn == null)
        {
            Debug.LogWarning("SpawnPoint nulo.");
            return;
        }

        GameManager gm = FindFirstObjectByType<GameManager>();

        if (gm == null)
        {
            Debug.LogWarning("No se encontró un GameManager en la escena.");
            return;
        }

        gm.InstanciarMonstruo(spawn);
    }

    private void InstanciarMonstruo(SpawnPoint spawn)
    {
        if (monstruoPrefab == null)
        {
            Debug.LogWarning("No hay monstruoPrefab asignado.");
            return;
        }

        if (spawn.spawnPosition == null)
        {
            Debug.LogWarning($"El SpawnPoint '{spawn.name}' no tiene SpawnPosition asignado.");
            return;
        }

        Transform punto = spawn.spawnPosition;

        GameObject monstruoObj = Instantiate(
            monstruoPrefab,
            punto.position,
            punto.rotation
        );

        if (ScreamerSound != null)
        {
            AudioSource.PlayClipAtPoint(
                ScreamerSound,
                punto.position,
                VolumeScreamer
            );
        }

        Debug.Log($"Monstruo instanciado en {spawn.name}");

        OnMonstruoSpawnea?.Invoke(monstruoObj);
    }

    public static void DispararVictoria()
    {
        OnGameWin?.Invoke();
    }

    public static void DispararDerrota()
    {
        OnGameLose?.Invoke();
    }
}