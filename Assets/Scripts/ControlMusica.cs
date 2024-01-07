using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMusica : MonoBehaviour
{
    AudioSource audioSource;

    public Sprite iconoSonidoActivado;
    public Sprite iconoMute;

    private bool estaMute = false;
    private Image imagen;

    void Start()
    {
        imagen = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (audioSource.isPlaying)
                    audioSource.Pause();
                else
                    audioSource.UnPause();
                ToggleMute();
            }
    }
    public void OnClick()
    {
        ToggleMute();
    }
    private void ToggleMute()
    {
        estaMute = !estaMute;
        imagen.sprite = estaMute ? iconoMute : iconoSonidoActivado;
        if (estaMute)
            audioSource.Pause();

        else
            audioSource.UnPause();
    }
}