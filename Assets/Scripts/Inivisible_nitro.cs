using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Inivisible_nitro : MonoBehaviour
{
    private Renderer rend;
    public float tiempoAparicionNitro = 5.0f;
    //public ControlVisibilidadTexto controlTexto;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            rend.enabled = false; //Hace desaparecer el nitro.
            //controlTexto.HacerVisible();
            Invoke("AparecerNitro", tiempoAparicionNitro);  // Invoca la función "Aperecer Nitro" después del tiempo especificado
        }
    }
    void AparecerNitro() //Hace aparecer el nitro.
    {
        rend.enabled = true;
        //controlTexto.HacerInvisible();
    }

}
