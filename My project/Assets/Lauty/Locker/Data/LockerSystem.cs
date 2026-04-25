using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockerSystem : ObjetoInteractivoBase
{
    [Header("Puntos de Teletransportacion")]
    public Transform insidePosition;
    public Transform outsidePosition;

    private bool isPlayerInside = false;
    private GameObject currentPlayer;

    [Header("Referencias de Sistemas")]
    public SanityManager sanityManager; // Arrastrá aquí el objeto que tiene la cordura

    [Header("Configuración de Pánico")]
    public float umbralRespiracion = 75f; // El límite que querés (70, 80, etc.)

    [Header("Efectos de Sonido")]
    public AudioSource audioSourceJugador;
    public AudioClip respiracionCalma;

    [Header("Referencias de UI")]
    public GameObject interactPrompt;
    public TextMeshProUGUI textoDelPrompt;

    [Header("Referencias del Jugador")]
    private CharacterController characterController;
    private PlayerControllerScript movement;

    // REEMPLAZÁ "MouseLook" POR EL NOMBRE EXACTO DE TU SCRIPT SI SE LLAMA DISTINTO
    public MouseLook scriptDeMirada;

    // Guardamos en qué frame entramos para evitar el bug del doble click
    private int frameDeEntrada = 0;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // Truco del Frame: Solo escuchamos el click si ya pasó el frame en el que entramos
        if (isPlayerInside && Time.frameCount > frameDeEntrada && Input.GetMouseButtonDown(0))
        {
            ExitLocker();
        }
    }

    void LateUpdate()
    {
        // Forzamos la UI a estar prendida después de que el jugador la haya apagado
        if (isPlayerInside)
        {
            if (interactPrompt != null) interactPrompt.SetActive(true);
            if (textoDelPrompt != null) textoDelPrompt.text = "Salir";
        }
    }

    public override string ObtenerMensaje()
    {
        return isPlayerInside ? "Salir" : "Esconderse";
    }

    public override void Interact(GameObject player)
    {
        if (!isPlayerInside)
        {
            EnterLocker(player);
        }
    }

    private void EnterLocker(GameObject player)
    {
        currentPlayer = player;
        isPlayerInside = true;

        // Congelamos el número de frame exacto de este momento
        frameDeEntrada = Time.frameCount;

        movement = currentPlayer.GetComponent<PlayerControllerScript>();
        characterController = currentPlayer.GetComponent<CharacterController>();

        // Apagamos físicas y movimiento para no rebotar
        if (characterController != null) characterController.enabled = false;
        if (movement != null) movement.enabled = false;

        // Le avisamos a TU script de cámara que active los límites
        if (scriptDeMirada != null) scriptDeMirada.ActivarModoLocker();

        currentPlayer.transform.position = insidePosition.position;
        currentPlayer.transform.rotation = insidePosition.rotation;
        
       
            
            if (sanityManager != null)
            {
                sanityManager.isHidden = true;

                // 2. Revisamos si está asustado para poner el audio
                if (sanityManager.currentSanity <= umbralRespiracion)
                {
                    if (audioSourceJugador != null && respiracionCalma != null)
                    {
                        audioSourceJugador.PlayOneShot(respiracionCalma);
                    }
                }
            }
        

        ShadowAI monstruo = Object.FindFirstObjectByType<ShadowAI>();

         if (monstruo != null)
         {
             // Le avisamos que nos escondimos y le pasamos el Transform de este casillero
             monstruo.OnPlayerHidden(this.transform);
         }

    }

    private void ExitLocker()
    {
        if (currentPlayer == null) return;

        currentPlayer.transform.position = outsidePosition.position;

        // Prendemos físicas y movimiento de nuevo
        if (characterController != null) characterController.enabled = true;
        if (movement != null) movement.enabled = true;

        // Le avisamos a TU script de cámara que vuelva a la normalidad
        if (scriptDeMirada != null) scriptDeMirada.DesactivarModoLocker();

        isPlayerInside = false;
        currentPlayer = null;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (sanityManager != null)
        {
            sanityManager.isHidden = false;
        }
            
        if (audioSourceJugador != null && respiracionCalma != null)
        { 
                audioSourceJugador.Stop();
        }
    }
}