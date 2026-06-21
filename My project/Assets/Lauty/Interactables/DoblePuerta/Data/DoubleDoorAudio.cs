using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoubleDoorAudio : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioSource audioCentral;
    public AudioClip slamSound;
    public float delayTime = 0.2f;

    private bool hasPlayed = false;
    private HashSet<Collider> touching = new HashSet<Collider>();

    void OnCollisionEnter(Collision collision)
    {
        // Evita que suene apenas arranca el juego si la puerta toca el piso
        if (Time.time < 0.5f) return;

        if (slamSound != null && audioCentral != null)
        {
            touching.Add(collision.collider);
            StartCoroutine(DelayedPlay(collision.collider));
        }
    }

    void OnCollisionExit(Collision collision)
    {
        touching.Remove(collision.collider);
        if (touching.Count == 0) hasPlayed = false; // permitir futuros slams
    }

    IEnumerator DelayedPlay(Collider col)
    {
        yield return new WaitForSeconds(delayTime);

        // Solo reproducir si sigue en contacto y no se ha reproducido ya
        if (!hasPlayed && touching.Contains(col))
        {
            // Le decimos al parlante central que suene
            audioCentral.PlayOneShot(slamSound);
            hasPlayed = true;
        }
    }
}