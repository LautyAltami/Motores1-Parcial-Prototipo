using UnityEngine;
using System.Collections;
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    public Transform playerBody;

    float xRotation = 0f;
    bool firstFrame = true;

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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
