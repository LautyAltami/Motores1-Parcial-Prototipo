using UnityEngine;

public class EmpujadorPuertaDobles : MonoBehaviour
{
    [Header("Configuración de Empuje")]
    public float fuerzaImpacto = 20f;
    public string tagPuertaDoble = "DoubleDoor"; // Asegurate de ponerle este tag a las puertas

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Solo interactuamos si el objeto tiene Rigidbody y el tag correcto
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null && hit.gameObject.CompareTag(tagPuertaDoble))
        {
            // Calculamos la dirección del empujón (hacia adelante del jugador)
            Vector3 direccion = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            // Aplicamos un impulso fuerte
            rb.AddForceAtPosition(direccion * fuerzaImpacto, hit.point, ForceMode.Impulse);

            // OPCIONAL: Si querés que al tocar una se abra la otra automáticamente,
            // podemos buscar si la puerta tiene un script de 'vínculo'.
        }
    }
}