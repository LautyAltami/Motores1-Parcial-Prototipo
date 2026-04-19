using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        // Solo para que vos puedas ver el rayo en la pestaña "Scene" mientras testeás
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.yellow);

        // Si apretamos la E, disparamos el rayo invisible desde el centro de la cámara
        if (Input.GetKeyDown(interactKey))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance))
            {
                // Si el rayo golpea algo que tiene el Tag "Locker"
                if (hit.collider.CompareTag("Locker"))
                {
                    // Buscamos el script del locker y le decimos que nos deje entrar
                    LockerSystem locker = hit.collider.GetComponent<LockerSystem>();
                    if (locker != null)
                    {
                        // Le pasamos el objeto raíz (el jugador completo, no solo la cámara)
                        locker.EnterLocker(transform.root.gameObject);
                    }
                }

                // NOTA FUTURA: Acá mismo podés agregar:
                // else if (hit.collider.CompareTag("Key")) { ... agarrar llave ... }
            }
        }
    }
}