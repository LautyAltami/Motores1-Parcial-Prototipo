using UnityEngine;

public interface IInteractable
{
    // La accion de interactuar (entrar al locker, agarrar llave, etc.)
    void Interact(GameObject player);

    // El texto que va a aparecer flotando
    string ObtenerMensaje();
    void EncenderBrillo();
    void ApagarBrillo();
}