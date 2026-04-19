using UnityEngine;

public class LockerSystem : MonoBehaviour
{
    [Header("Teleport Points")]
    public Transform insidePosition;
    public Transform outsidePosition;

    private bool isPlayerInside = false;
    private GameObject currentPlayer; // Guarda quién entró

    // Evita que una única pulsación de E dispare entrar->salir o salir->entrar en el mismo frame
    private bool requireKeyRelease = false;

    void Update()
    {
        // Si estamos esperando la liberación de la tecla E, solo la limpiamos al soltarla
        if (requireKeyRelease)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                requireKeyRelease = false;
            }

            // No procesamos salidas mientras esperamos el release
            return;
        }

        // Si el jugador YA ESTÁ ADENTRO, la única forma de salir es apretar la E
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            ExitLocker();
        }
    }

    // Esta función PÚBLICA es la que llama el Raycast del jugador
    public void EnterLocker(GameObject player)
    {
        // Protección extra: si ya estamos dentro o estamos bloqueando interacciones por release, no entrar
        if (isPlayerInside) return;
        if (requireKeyRelease) return;

        currentPlayer = player;
        isPlayerInside = true;

        // Bloqueamos hasta que se suelte la tecla para evitar toggle en el mismo press
        requireKeyRelease = true;

        // Avisamos a la cordura que estamos escondidos
        SanityManager sanity = currentPlayer != null ? currentPlayer.GetComponent<SanityManager>() : null;
        if (sanity != null) sanity.isHidden = true;

        var movement = currentPlayer != null ? currentPlayer.GetComponent<PlayerMovement>() : null;
        if (movement != null) movement.enabled = false;

        // Teletransportar adentro
        currentPlayer.transform.position = insidePosition.position;
        currentPlayer.transform.rotation = insidePosition.rotation;

        Debug.Log("Entraste al locker mediante Raycast.");
    }

    private void ExitLocker()   
    {
        // Seguridad: si no hay jugador, solo aseguramos estado
        if (currentPlayer == null)
        {
            isPlayerInside = false;
            return;
        }

        isPlayerInside = false;

        // Evitamos que la misma pulsación vuelva a abrir o hacer otra interacción
        requireKeyRelease = true;

        // Avisamos a la cordura que salimos
        SanityManager sanity = currentPlayer.GetComponent<SanityManager>();
        if (sanity != null) sanity.isHidden = false;

        var movement = currentPlayer.GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = true;

        // Teletransportar afuera
        currentPlayer.transform.position = outsidePosition.position;

        currentPlayer = null; // Vaciamos la referencia
        Debug.Log("Saliste del locker.");
    }
}