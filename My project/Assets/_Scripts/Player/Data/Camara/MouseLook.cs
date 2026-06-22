using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    public Transform playerBody;

    [Header("Limites del Casillero")]
    public float limiteHorizontal = 60f;
    public float limiteVertical = 45f; // Un poco menos de 90 para no mirar tu propio ombligo o el techo

    float xRotation = 0f;
    bool firstFrame = true;

    // Variables internas para el casillero
    private bool enLocker = false;
    private float yRotationLocker = 0f; // Guarda la rotacion del cuello a los lados

    IEnumerator Start()
    {
        // Bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Espera 1 frame para evitar input fantasma
        yield return null;
        xRotation = 0f;
        transform.localRotation = Quaternion.identity;
    }

    // Funciones publicas que llama el LockerSystem
    public void ActivarModoLocker()
    {
        enLocker = true;
        yRotationLocker = 0f; // Centramos el cuello al entrar
    }

    public void DesactivarModoLocker()
    {
        enLocker = false;
        // Al salir, forzamos a que el cuello vuelva a mirar derecho respecto al cuerpo
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Update()
    {
        // Ignora el primer frame de Update
        if (firstFrame)
        {
            firstFrame = false;
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (!enLocker)
        {
            // --- MODO EXPLORACION (Normal) ---
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Gira el cuello arriba/abajo
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // Gira el cuerpo a los lados
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // --- MODO CASILLERO (Encerrado) ---
            xRotation -= mouseY;
            yRotationLocker += mouseX; // Aca sumamos el X del mouse a la variable del cuello

            // Recortamos (Clamp) ambas direcciones
            xRotation = Mathf.Clamp(xRotation, -limiteVertical, limiteVertical);
            yRotationLocker = Mathf.Clamp(yRotationLocker, -limiteHorizontal, limiteHorizontal);

            // Le aplicamos TANTO arriba/abajo como izquierda/derecha SOLO a la camara
            transform.localRotation = Quaternion.Euler(xRotation, yRotationLocker, 0f);
        }
    }
}