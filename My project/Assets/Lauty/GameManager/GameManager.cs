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
        // Seguridad: verificamos que el índice exista en la lista para evitar crasheos
        if (indice >= 0 && indice < puntosDeSpawn.Count)
        {
            Transform puntoElegido = puntosDeSpawn[indice];

            // Instanciamos el monstruo en la posición y rotación del punto elegido
            Instantiate(monstruoPrefab, puntoElegido.position, puntoElegido.rotation);
            AudioSource.PlayClipAtPoint(ScreamerSound, puntoElegido.position, VolumeScreamer);
            Debug.Log("Monstruo instanciado con éxito en el punto ID: " + indice);
        }
        else
        {
            Debug.LogWarning("El ID de spawn " + indice + " no existe en la lista del GameManager.");
        }
    }
}