using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class DoubleDoorAudio : MonoBehaviour
{
    public AudioClip slamSound;
    public float delayTime = 0.2f;

    private AudioSource audioSource;
    private bool hasPlayed = false;
    private HashSet<Collider> touching = new HashSet<Collider>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time < 0.5f) return;

        if (slamSound != null)
        {
            touching.Add(collision.collider);
            StartCoroutine(DelayedPlay(collision.collider));
        }
    }

    void OnCollisionExit(Collision collision)
    {
        touching.Remove(collision.collider);
        if (touching.Count == 0) hasPlayed = false; // permitir futuros slams después de que termine el contacto
    }

    IEnumerator DelayedPlay(Collider col)
    {
        yield return new WaitForSeconds(delayTime);

        // Solo reproducir si el mismo collider sigue en contacto y no se ha reproducido ya
        if (!hasPlayed && touching.Contains(col))
        {
            audioSource.PlayOneShot(slamSound);
            hasPlayed = true;
        }
    }
}   