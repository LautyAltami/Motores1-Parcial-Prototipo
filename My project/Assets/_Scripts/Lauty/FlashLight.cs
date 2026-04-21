using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Necesario para la imagen del cooldown

public class Flashlight : ObjetoInteractivoBase
{
    [Header("Estado de la Linterna")]
    public bool estaEquipada = false;
    public Transform posicionEnMano; // Objeto vacío hijo de tu cámara para saber dónde ponerla

    [Header("Linterna Normal")]
    public Light luzSpotlight;
    public AudioSource audioSource;
    public AudioClip sonidoClick;
    public AudioClip sonidoFlash;

    [Header("Cooldown y Aturdimiento")]
    public float intensidadFlash = 15f;
    public float cooldownTotal = 5f;
    private float timerCooldown = 0f;
    public float distanciaStun = 10f;
    public float radioDelCono = 1.5f;

    [Header("Feedback Visual")]
    public Image imagenCooldownUI;
    public GameObject prefabImpacto;
    public TextMeshProUGUI textoControlesLinterna;

    [TextArea] // Este atributo te deja escribir en un cuadro más grande en el Inspector
    public string mensajeControles = "[F] Linterna   |   [Click Der] Destello";

    protected override void Start()
    {
        base.Start(); // Busca el Outline automáticamente
        if (luzSpotlight != null) luzSpotlight.enabled = false;
    }

    // 1. Mensaje para la UI de tu Raycast
    public override string ObtenerMensaje()
    {
        // Si ya la tenés en la mano, no devuelve mensaje. Si está en la mesa, sí.
        return estaEquipada ? "" : "Recoger Linterna";
    }

    // 2. Cuando le hacés click con tu sistema de interacción
    public override void Interact(GameObject player)
    {
        if (!estaEquipada)
        {
            Equipar(player);
        }
    }

    private void Equipar(GameObject player)
    {
        estaEquipada = true;

        // Apagamos el collider y el outline para que no molesten en la mano
        GetComponent<Collider>().enabled = false;
        ApagarBrillo(); // Función nativa de tu ObjetoInteractivoBase

        // Emparentamos la linterna a la cámara del jugador
        Camera cam = player.GetComponentInChildren<Camera>();
        transform.SetParent(cam.transform);

        // La acomodamos en la pantalla
        if (posicionEnMano != null)
        {
            transform.position = posicionEnMano.position;
            transform.rotation = posicionEnMano.rotation;
        }
        else
        {
            // Posición por defecto si te olvidás de asignar el Transform
            transform.localPosition = new Vector3(0.4f, -0.4f, 0.8f);
            transform.localRotation = Quaternion.identity;
        }
        if (textoControlesLinterna != null)
        {
            textoControlesLinterna.text = mensajeControles;
        }
    }

    void Update()
    {
        if (!estaEquipada) return; // Si sigue en la mesa, cortamos acá

        // --- SISTEMA DE COOLDOWN VISUAL ---
        if (timerCooldown > 0)
        {
            timerCooldown -= Time.deltaTime;

            // Actualizamos la UI (0 es vacío, 1 es lleno)
            if (imagenCooldownUI != null)
            {
                imagenCooldownUI.fillAmount = 1f - (timerCooldown / cooldownTotal);
            }
        }

        // --- PRENDER / APAGAR (F) ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            luzSpotlight.enabled = !luzSpotlight.enabled;
            if (audioSource != null && sonidoClick != null) audioSource.PlayOneShot(sonidoClick);
        }

        // --- DISPARO DEL FLASH (Click Derecho) ---
        if (Input.GetMouseButtonDown(1) && luzSpotlight.enabled && timerCooldown <= 0f)
        {
            StartCoroutine(LanzarFlash());
        }
    }

    private IEnumerator LanzarFlash()
    {
        // Reseteamos tiempos y vaciamos la UI
        timerCooldown = cooldownTotal;
        if (imagenCooldownUI != null) imagenCooldownUI.fillAmount = 0f;

        // Efecto visual propio de la linterna
        float luzOriginal = luzSpotlight.intensity;
        luzSpotlight.intensity = intensidadFlash;
        if (audioSource != null && sonidoFlash != null) audioSource.PlayOneShot(sonidoFlash);

        // --- EL DISPARO Y EL IMPACTO ---
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, radioDelCono, transform.forward, out hit, distanciaStun))
        {
            // INSTANTIATE: Creamos el destello justo donde chocó el rayo
            if (prefabImpacto != null)
            {
                // Instanciamos rotando hacia donde mira la normal (la pared)
                GameObject destello = Instantiate(prefabImpacto, hit.point, Quaternion.LookRotation(hit.normal));
                // Destruimos la luz/partícula después de medio segundo para no llenar la memoria
                Destroy(destello, 0.5f);
            }

            // TODO: Descomentar esto cuando unas el proyecto con tu compañero
            /*
            ShadowAI monstruo = hit.collider.GetComponentInParent<ShadowAI>();
            if (monstruo != null)
            {
                monstruo.Stun(2.5f);
                Debug.Log("¡Le pegaste al monstruo!");
            }
            */
        }

        // Esperamos el pantallazo blanco y volvemos a la normalidad
        yield return new WaitForSeconds(0.2f);
        luzSpotlight.intensity = luzOriginal;
    }
}