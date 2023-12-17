using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisibleInivisibleTexto : MonoBehaviour
{
    public TextMeshProUGUI texto;

    // Método para hacer visible el texto
    public void HacerVisible()
    {
        if (texto != null)
        {
            texto.enabled = true;
        }
    }

    // Método para hacer invisible el texto
    public void HacerInvisible()
    {
        if (texto != null)
        {
            texto.enabled = false;
        }
    }
}
