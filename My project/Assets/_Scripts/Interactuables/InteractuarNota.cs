using UnityEngine;
using UnityEngine.UI; // Necesario para interactuar con el Canvas

public class InteractuarNota : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject canvasNota; // Asigna aquí el Canvas o Panel que contiene la nota nítida
    public float distanciaInteraccion = 2.0f; // Distancia máxima para interactuar

    private bool estaMostrandoNota = false;
    private Transform camaraJugador;

    void Start()
    {
        // Encuentra la cámara principal (normalmente la del jugador)
        camaraJugador = Camera.main.transform;

        // Asegurarse de que el canvas empiece desactivado
        if (canvasNota != null)
        {
            canvasNota.SetActive(false);
        }
    }

    void Update()
    {
        // Solo verificamos la interacción si el jugador está cerca y no se está mostrando la nota
        if (!estaMostrandoNota && Vector3.Distance(transform.position, camaraJugador.position) < distanciaInteraccion)
        {
            // Opción A: Clic izquierdo del ratón
            if (Input.GetMouseButtonDown(0))
            {
                // Usamos un Raycast para verificar si el clic fue sobre este objeto
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        MostrarNota();
                    }
                }
            }

            // Opción B: Presionar la tecla 'E'
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Similar al Raycast del clic, pero lanzado desde el centro de la cámara
                Ray ray = new Ray(camaraJugador.position, camaraJugador.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distanciaInteraccion))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        MostrarNota();
                    }
                }
            }
        }
        else if (estaMostrandoNota)
        {
            // Opción para cerrar la nota (por ejemplo, clic o 'E' de nuevo, o 'Esc')
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                OcultarNota();
            }
        }
    }

    // Funciones para mostrar y ocultar la nota en el Canvas
    void MostrarNota()
    {
        if (canvasNota != null)
        {
            canvasNota.SetActive(true);
            estaMostrandoNota = true;
            // Opcional: Pausar el juego o bloquear movimiento del jugador aquí
        }
    }

    void OcultarNota()
    {
        if (canvasNota != null)
        {
            canvasNota.SetActive(false);
            estaMostrandoNota = false;
            // Opcional: Reanudar el juego o movimiento del jugador aquí
        }
    }
}