using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    [Tooltip("El ID debe coincidir con el orden en la lista del GameManager (0, 1, 2...)")]
    public int idDeEsteSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Le gritamos al GameManager pasándole nuestro ID específico
            GameManager.DispararSpawn(idDeEsteSpawn);

            // Destruimos este trigger para que no genere monstruos infinitos al volver a pisarlo
            Destroy(gameObject);
        }
    }
}