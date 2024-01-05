using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameConnection : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clasificacion;
    public Sprite spriteBlanco, spriteRojo, spriteAzul, spriteAmarillo, spriteVerde, spriteMorado;
    private Sprite spriteSeleccionado;
    public GameObject otherPlayerPrefab;
    public GameObject gameArea;
    Socket server = PantallaPrincipal.server;
    ConcurrentQueue<string> queue = PantallaPrincipal.responseQueue;
    Dictionary<string, GameObject> jugadoresEnPartida = new Dictionary<string, GameObject>();
    int posicion=0;
    int color;

    void Start()
    {
        InvokeRepeating("SendPlayerInformation", 0f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        while(queue.Count > 0) {
            queue.TryDequeue(out string respuesta);
            EjecutarRespuesta(respuesta);
        }
    }

    void SendPlayerInformation()
    {
        if (server == null || !server.Connected) return;

        string x = this.transform.position.x.ToString("0.0000", CultureInfo.InvariantCulture);
        string y = this.transform.position.y.ToString("0.0000", CultureInfo.InvariantCulture);
        string rot = this.transform.rotation.eulerAngles.z.ToString("0.0000", CultureInfo.InvariantCulture);
        string name = PantallaPrincipal.usuario;
        int idPartida = PantallaPrincipal.idPartida;
        if(ScenesManager.colorPrincipal.Equals(spriteRojo))
            color=0;
        else if(ScenesManager.colorPrincipal.Equals(spriteAmarillo))
            color=1;
        else if(ScenesManager.colorPrincipal.Equals(spriteVerde))
            color=2;
        else if(ScenesManager.colorPrincipal.Equals(spriteAzul))
            color=3;
        else if(ScenesManager.colorPrincipal.Equals(spriteMorado))
            color=4;
        else if(ScenesManager.colorPrincipal.Equals(spriteBlanco))
            color=5;
        string mensaje = $"14/{idPartida}/{name}/{x}/{y}/{rot}/{color}";
        byte[] bytes = Encoding.ASCII.GetBytes(mensaje);
        server.Send(bytes);
    }

    void EjecutarRespuesta(string respuesta)
    {
        string[] trozos = respuesta.Split(new[] { '/' }, 2);
        int codigo = Convert.ToInt32(trozos[0]);
        string mensaje = trozos[1];

        switch(codigo){
            case 14:
                ActualizarPosicionJugadores(mensaje);
                break;
            case 18:
                MostrarTiempos(mensaje);
                break;
            case 19:

                break;
        }
    }

    void ActualizarPosicionJugadores(string mensaje)
    {
        string[] trozos = mensaje.Split('/');
        int i = 0;
        while (i < trozos.Length)
        {
            string nombre = trozos[i];
            float x = float.Parse(trozos[i + 1], CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(trozos[i + 2], CultureInfo.InvariantCulture.NumberFormat);
            float rot = float.Parse(trozos[i + 3], CultureInfo.InvariantCulture.NumberFormat);
            int colorSeleccionado = int.Parse(trozos[i + 4], CultureInfo.InvariantCulture.NumberFormat);
            if(colorSeleccionado==0)
                spriteSeleccionado=spriteRojo;
            else if(colorSeleccionado==1)
                spriteSeleccionado=spriteAmarillo;
            else if(colorSeleccionado==2)
                spriteSeleccionado=spriteVerde;
            else if(colorSeleccionado==3)
                spriteSeleccionado=spriteAzul;
            else if(colorSeleccionado==4)
                spriteSeleccionado=spriteAzul;
            else if(colorSeleccionado==5)
                spriteSeleccionado=spriteBlanco;
            if (jugadoresEnPartida.ContainsKey(nombre))
            {
                jugadoresEnPartida[nombre].transform.position = new Vector2(x, y);
                jugadoresEnPartida[nombre].transform.rotation = Quaternion.Euler(0, 0, rot);
                jugadoresEnPartida[nombre].transform.Find("Name").rotation = Quaternion.Euler(0, 0, 0);
                jugadoresEnPartida[nombre].GetComponent<SpriteRenderer>().sprite=spriteSeleccionado;
            }
            else
            {
                GameObject jugador = Instantiate(otherPlayerPrefab, new Vector2(x, y), Quaternion.identity);
                jugador.transform.SetParent(gameArea.transform, false);
                jugador.transform.Find("Name/Name").GetComponent<Text>().text = nombre;
                jugador.transform.rotation = Quaternion.Euler(0, 0, rot);
                jugador.transform.Find("Name").rotation = Quaternion.Euler(0, 0, 0);
                jugadoresEnPartida.Add(nombre, jugador);
            }

            i += 5;

        }
    }
    void MostrarTiempos(string mensaje)
    {
        string[] trozos = mensaje.Split('/');
        string nombre = trozos[0];
        float tiempo = float.Parse(trozos[1], CultureInfo.InvariantCulture.NumberFormat);
        float mejorTiempo = float.Parse(trozos[2], CultureInfo.InvariantCulture.NumberFormat);
        posicion ++;
        clasificacion.text += posicion.ToString() + ".\t" + nombre + ":\t" + tiempo + "\t" + mejorTiempo + "\n";
    }
}
