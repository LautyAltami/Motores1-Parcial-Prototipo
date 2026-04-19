using UnityEngine;
using UnityEngine.UI;
public class PlayerControllerScript : MonoBehaviour
{
    [Header("MovementConfig")]
    [SerializeField] float moveVelocity;
    Vector3 moveAxis, moveDir;
    CharacterController characterController;
    [Header("CameraConfig")]
    [SerializeField] float sensitivity;
    [SerializeField] GameObject cameraObject;
    Vector3 camFoward, camRight;
    Vector3 camRot;
    [Header("GravityConfig")]
    [SerializeField] float gravity;
    float fallVelocity;
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


    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseDisable();
        Interact();
        Inputs();
        SetGravity();
        Movement(); //Movement siempre irá a lo ultimo
    }
    private void FixedUpdate()
    {

    }
    public void Inputs()
    {
        moveAxis = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (moveDir.magnitude > 1) moveDir = moveDir.normalized;

        camFoward = cameraObject.transform.forward.normalized;
        camFoward.y = 0;
        camRight = cameraObject.transform.right.normalized;
        camRight.y = 0;

        moveDir = moveAxis.x * camRight + moveAxis.z * camFoward;
        moveDir *= Time.deltaTime;

        Vector3 camRotInput = new(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
        camRot +=  sensitivity * Time.deltaTime * camRotInput;
        camRot.x = Mathf.Clamp(camRot.x, -60f, 60f);
    }
    public void Movement()
    {
        characterController.Move(moveDir * moveVelocity);

        cameraObject.transform.localRotation = Quaternion.Euler(camRot);
    }
    void SetGravity()
    {
        if (characterController.isGrounded)
        {
            fallVelocity = -gravity * Time.deltaTime;
        }
        else
        {
            fallVelocity -= gravity * Time.deltaTime;
        }
        moveDir.y = fallVelocity;
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

        if (Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, interactionRange, interactuable))
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
