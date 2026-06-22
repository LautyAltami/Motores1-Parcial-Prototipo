using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuracion")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public Transform playerRoot;

    [Header("UI Pantalla (HUD)")]
    public Image puntoMira;
    public GameObject interactPrompt;
    public TextMeshProUGUI textoDelPrompt;

    private Camera mainCamera;
    private Outline ultimoOutline;

    void Start()
    {
        mainCamera = Camera.main;
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (playerRoot == null) playerRoot = transform.root;

        // Lo forzamos a blanco al arrancar por las dudas y no lo tocamos mas
        if (puntoMira != null) puntoMira.color = Color.white;
    }

    private IInteractable ultimoObjetoMirado;

    void Update()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable objetoActual = hit.collider.GetComponentInParent<IInteractable>();

            if (objetoActual != null)
            {
                // Si cambié de objeto, apago el anterior y prendo el nuevo
                if (objetoActual != ultimoObjetoMirado)
                {
                    if (ultimoObjetoMirado != null) ultimoObjetoMirado.ApagarBrillo();

                    ultimoObjetoMirado = objetoActual;
                    ultimoObjetoMirado.EncenderBrillo();
                }
                string mensaje = objetoActual.ObtenerMensaje();
                if (mensaje != "")
                {
                    interactPrompt.SetActive(true);
                    textoDelPrompt.text = mensaje;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    objetoActual.Interact(playerRoot.gameObject);
                }

                return;
                
            }
        }

        // RESET: Si el rayo no toca nada, apago el último que guardé
        if (ultimoObjetoMirado != null)
        {
            ultimoObjetoMirado.ApagarBrillo();
            ultimoObjetoMirado = null;
        }

        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}