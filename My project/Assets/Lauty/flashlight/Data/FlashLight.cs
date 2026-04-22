using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : ObjetoInteractivoBase
{
    [Header("Estado de la Linterna")]
    public bool estaEquipada = false;
    public Transform posicionEnMano;

    [Header("Conexión con Sanidad")]
    public SanityManager sanityManager; // <-- Conexión para avisar si hay luz

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
    public TextMeshProUGUI textoControlesLinterna;

    [TextArea]
    public string mensajeControles = "[F] Linterna   |   [Click Der] Destello";

    protected override void Start()
    {
        base.Start();
        if (luzSpotlight != null) luzSpotlight.enabled = false;

        if (imagenCooldownUI != null)
        {
            imagenCooldownUI.gameObject.SetActive(false);
        }
    }

    public override string ObtenerMensaje()
    {
        return estaEquipada ? "" : "Linterna";
    }

    public override void Interact(GameObject player)
    {
        if (!estaEquipada) Equipar(player);
    }

    private void Equipar(GameObject player)
    {
        estaEquipada = true;
        GetComponent<Collider>().enabled = false;
        ApagarBrillo();

        Camera cam = player.GetComponentInChildren<Camera>();
        transform.SetParent(cam.transform);

        if (posicionEnMano != null)
        {
            transform.position = posicionEnMano.position;
            transform.rotation = posicionEnMano.rotation;
        }
        else
        {
            transform.localPosition = new Vector3(0.4f, -0.4f, 0.8f);
            transform.localRotation = Quaternion.identity;
        }

        if (textoControlesLinterna != null)
        {
            textoControlesLinterna.text = mensajeControles;
        }
        if (imagenCooldownUI != null)
        {
            imagenCooldownUI.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!estaEquipada) return;

        // --- SISTEMA DE COOLDOWN VISUAL ---
        if (timerCooldown > 0)
        {
            timerCooldown -= Time.deltaTime;
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

            // Le avisamos al SanityManager si estamos iluminando o a oscuras
            if (sanityManager != null)
            {
                sanityManager.hasLight = luzSpotlight.enabled;
            }
        }

        // --- DISPARO DEL FLASH (Click Derecho) ---
        if (Input.GetMouseButtonDown(1) && luzSpotlight.enabled && timerCooldown <= 0f)
        {
            StartCoroutine(LanzarFlash());
        }
    }

    private IEnumerator LanzarFlash()
    {
        // 1. Reseteamos tiempos y vaciamos la UI
        timerCooldown = cooldownTotal;
        if (imagenCooldownUI != null) imagenCooldownUI.fillAmount = 0f;

        // 2. Efecto visual y sonoro (El flashazo)
        float luzOriginal = luzSpotlight.intensity;
        luzSpotlight.intensity = intensidadFlash;
        if (audioSource != null && sonidoFlash != null) audioSource.PlayOneShot(sonidoFlash);

        // 3. Detectar si le pegamos al monstruo (SphereCast)
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, radioDelCono, transform.forward, out hit, distanciaStun))
        {
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

        // 4. Esperamos 0.2 segundos y devolvemos la luz a la normalidad
        yield return new WaitForSeconds(0.2f);
        luzSpotlight.intensity = luzOriginal;
    }
}