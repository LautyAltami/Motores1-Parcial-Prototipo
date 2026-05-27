using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Configuración del Monstruo")]
    public GameObject monstruoPrefab; // Arrastrá el prefab del monstruo acá

    [Header("Configuración de Audio")]
    public AudioClip ScreamerSound;
    public float VolumeScreamer = 1.0f;
    public AudioSource musicaTension;

    [Header("Puntos de Aparición")]
    // Esta lista te permite agregar 3, 20 o 50 puntos desde el Inspector
    public List<Transform> puntosDeSpawn;


    // El evento ahora transporta un número entero (el ID del spawn)
    public static event Action<int> OnMonstruoSpawnea;

    // Eventos para ganar o perder el juego (pueden ser llamados desde otros scripts)
    public static event Action OnGameWin;
    public static event Action OnGameLose;

    // Método que llaman los triggers
    public static void DispararSpawn(int idSpawn)
    {
        OnMonstruoSpawnea?.Invoke(idSpawn);
    }

    private void OnEnable()
    {
        OnMonstruoSpawnea += InstanciarMonstruo;
    }

    private void OnDisable()
    {
        OnMonstruoSpawnea -= InstanciarMonstruo;
    }

    private void InstanciarMonstruo(int indice)
    {
        if (indice >= 0 && indice < puntosDeSpawn.Count)
        {
            Transform puntoElegido = puntosDeSpawn[indice];

            GameObject monstruoObj = Instantiate(
                monstruoPrefab,
                puntoElegido.position,
                puntoElegido.rotation
            );

            AudioSource.PlayClipAtPoint(ScreamerSound, puntoElegido.position, VolumeScreamer);

            Debug.Log("Monstruo instanciado y activado correctamente en ID: " + indice);
        }
        else
        {
            Debug.LogWarning("El ID de spawn " + indice + " no existe.");
        }
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