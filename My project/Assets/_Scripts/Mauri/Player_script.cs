using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
public class Player_script : MonoBehaviour
{
    [Header("Camera_config")]
    [SerializeField] GameObject Camera;
    float cameraRotationV;
    [Header("Movement_config")]
    [SerializeField] float player_velocity = 1;
    [SerializeField] float camerasensitivity;
    [Header("Jump_config")]
    [SerializeField] float jumpForce = 1f;
    [SerializeField] float rayCastDistanceJ;
    [SerializeField] LayerMask Ground;
    [Header("Interaction_config")]
    [SerializeField] float interactionRange = 1f;
    [SerializeField] LayerMask interactuable;
    [SerializeField] GameObject interactAbleText;
    [Header("Sanity_config")]
    [SerializeField] float initialSanityValue;
    [SerializeField] float maxSanityValue;
    [SerializeField] float minSanityValue;
    [SerializeField] float SanityLossRate;
    [SerializeField] Slider sanitySlider;
    float actualSanityvalue;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactAbleText.SetActive(false);
        actualSanityvalue = initialSanityValue;
        SanityInitialSlider();
    }
    private void Update()
    {
        CameraMovement();
        MouseDisable();
        Interact();
        SanityMechanic(0);
    }
    private void FixedUpdate()
    {
        Movement();
        Jump();
    }
    private void Jump()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayCastDistanceJ, Ground) && Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    void Movement()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float playerRotation = Input.GetAxis("Mouse X");   

        transform.Rotate(0f, playerRotation * camerasensitivity * Time.deltaTime, 0f);

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 movementDirection = transform.forward * inputZ + transform.right * inputX;
        movementDirection = movementDirection.normalized * player_velocity;
        movementDirection.y = rb.linearVelocity.y;

        rb.linearVelocity = movementDirection;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    void CameraMovement()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        cameraRotationV -= Input.GetAxis("Mouse Y") * camerasensitivity * Time.deltaTime;

        cameraRotationV = Mathf.Clamp(cameraRotationV, -100f, 100f);

        Camera.transform.localRotation = Quaternion.Euler(cameraRotationV, 0, 0);
    }
    void MouseDisable()
    {
        //Activo o desactivo la visibilidad del Mouse segun el teclado "F"

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    void Interact()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, interactionRange, interactuable))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactAbleText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
            else
            {
                Debug.Log("Interactable interface doesn't exist");
            }
        }
        else
        {
            interactAbleText.SetActive(false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down * rayCastDistanceJ;

        Gizmos.DrawLine(origin, origin + direction);

        Gizmos.color = Color.aquamarine;
        Gizmos.DrawRay(Camera.transform.position, Camera.transform.forward * interactionRange);
    }
    public void SanityMechanic(float sanityLoss)
    {
        /*
         Si sanityLoss es 0 el jugador perderá cordura segun el ratio de perdida "SanityLossRate" aunque condicionada por el tiempo.
         Esta funcion sirve tambien para caidas abruptas de la cordura, pero no está implementada por ahora.
         nota1: SanityMechanic no es utilizada para la ganancia de cordura
         nota2: es posible fusionar las tareas de SanityMechanic y SanityGain, en tal caso renombrar sanityLoss adecuadamente
         nota3: La mecanica de cordura funciona sin el intermediario de una interfaz, no es obligatorio agregarla pero es recomendable implementarlo
         */
        if (sanityLoss == 0 && actualSanityvalue > 0)
        {
            actualSanityvalue -= SanityLossRate * Time.deltaTime;
        }
        else
        {
            actualSanityvalue -= sanityLoss;
        }

        if (actualSanityvalue < 0)
        {
            Debug.Log("La cordura a caido a 0");
        }
        sanitySlider.value = actualSanityvalue;
        Debug.Log(actualSanityvalue); 
    }
    public void SanityGain(float gain)
    {
        // gain será la cantidad de cordura ganada, como, por ejemplo, al interactuar con el objeto medicine
        actualSanityvalue += gain;

        sanitySlider.value = actualSanityvalue;
    }
    public void SanityInitialSlider()
    {
        sanitySlider.minValue = minSanityValue;
        sanitySlider.maxValue = maxSanityValue;
    }
}
