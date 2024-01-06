using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMusica : MonoBehaviour
{
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.S)) // Pausar la música al presionar la tecla P
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.UnPause();
        }
    }
}
