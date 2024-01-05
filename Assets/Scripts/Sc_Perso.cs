using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class Sc_Perso : MonoBehaviour
{
    public Sprite colorSelected;
    public Sprite spriteBlanco, spriteRojo, spriteAzul, spriteAmarillo, spriteVerde, spriteMorado;
    private Button _botonRojo, _botonAzul, _botonAmarillo,_botonVerde, _botonMorado, _botonBlanco;
    private GameObject[] colorButtons;
    GameObject Rojo, Azul, Amarillo, Verde, Morado, Blanco;


    void Start()
    {
        Rojo = GameObject.Find("Rojo");
        _botonRojo = Rojo.GetComponent<Button>();
        Azul = GameObject.Find("Azul");
        _botonAzul = Azul.GetComponent<Button>();
        Amarillo = GameObject.Find("Amarillo");
        _botonAmarillo = Amarillo.GetComponent<Button>();
        Verde = GameObject.Find("Verde");
        _botonVerde = Verde.GetComponent<Button>();
        Morado = GameObject.Find("Morado");
        _botonMorado = Morado.GetComponent<Button>();
        Blanco = GameObject.Find("Blanco");
        _botonBlanco = Blanco.GetComponent<Button>();
        colorButtons = new GameObject[] { Rojo, Azul, Amarillo, Verde, Morado, Blanco };

        colorSelected = spriteBlanco;
        Blanco.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
        
        _botonRojo.onClick.AddListener(colorRojo);
        _botonAzul.onClick.AddListener(colorAzul);
        _botonAmarillo.onClick.AddListener(colorAmarillo);
        _botonVerde.onClick.AddListener(colorVerde);
        _botonMorado.onClick.AddListener(colorMorado);
        _botonBlanco.onClick.AddListener(colorBlanco);

    }
    void Update()
    {
        ScenesManager.colorPrincipal = colorSelected;
    }
    void colorRojo()
    {
        colorSelected = spriteRojo;
        foreach (GameObject colorButton in colorButtons)
        {
            colorButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Rojo.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
    }
    void colorAzul()
    {
        colorSelected = spriteAzul;
        foreach (GameObject  colorButton in colorButtons)
        {
            colorButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Azul.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
    }
    void colorAmarillo()
    {
        colorSelected = spriteAmarillo;
        foreach (GameObject colorButton in colorButtons)
        {
            colorButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Amarillo.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
    }
    void colorVerde()
    {
        colorSelected = spriteVerde;
        foreach (GameObject colorButton in colorButtons)
        {
            colorButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Verde.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
    }
    void colorMorado()
    {
        colorSelected = spriteMorado;
        foreach (GameObject colorButton in colorButtons)
        {
            colorButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Morado.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
    }
    void colorBlanco()
    {
        colorSelected = spriteBlanco;
        foreach (GameObject colorButton in colorButtons)
        {
            colorButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        Blanco.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 215, 0, 255);
    }
}
