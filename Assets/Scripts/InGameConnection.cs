using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameConnection : MonoBehaviour
{
    public GameObject otherPlayerPrefab;
    public GameObject gameArea;

    Socket server = PantallaPrincipal.server;
    ConcurrentQueue<string> queue = PantallaPrincipal.responseQueue;
    Dictionary<string, GameObject> jugadoresEnPartida = new Dictionary<string, GameObject>();
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
        string rot = this.transform.rotation.z.ToString("0.0000", CultureInfo.InvariantCulture);
        string name = PantallaPrincipal.usuario;
        int idPartida = PantallaPrincipal.idPartida;

        string mensaje = $"14/{idPartida}/{name}/{x}/{y}/{rot}";
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
            if (jugadoresEnPartida.ContainsKey(nombre))
            {
                jugadoresEnPartida[nombre].GetComponent<Transform>().position = new Vector2(x, y);
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

            i += 4;

        }
    }
}
