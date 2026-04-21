using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerScript : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] float moveVelocity = 5f;
    CharacterController characterController;

    [Header("Gravity Config")]
    [SerializeField] float gravity = 9.81f;
    float fallVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ARREGLO PARA EL LOCKER: Si el script está apagado (porque nos escondimos), no calcula gravedad ni movimiento
        if (!characterController.enabled) return;

        Movement();
        SetGravity();
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1) move = move.normalized;

        // Juntamos el movimiento horizontal con la gravedad vertical
        Vector3 finalVelocity = (move * moveVelocity) + (Vector3.up * fallVelocity);

        characterController.Move(finalVelocity * Time.deltaTime);
    }

    void SetGravity()
    {
        if (characterController.isGrounded)
        {
            // Un pequeño empuje hacia abajo constante para que el personaje no "flote" en bajadas
            fallVelocity = -2f;
        }
        else
        {
            fallVelocity -= gravity * Time.deltaTime;
        }
    }
}