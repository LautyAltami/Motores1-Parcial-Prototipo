using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class ObjetoInteractivoBase : MonoBehaviour, IInteractable
{
    protected Outline contorno;

    protected virtual void Start()
    {
        contorno = GetComponent<Outline>();
        if (contorno != null) contorno.enabled = false;
    }

    // Funciones que ganan todos los hijos
    public void EncenderBrillo() { if (contorno != null) contorno.enabled = true; }
    public void ApagarBrillo() { if (contorno != null) contorno.enabled = false; }

    // Estas las tiene que llenar cada hijo (Puerta, etc.)
    public abstract void Interact(GameObject player);
    public abstract string ObtenerMensaje();
}