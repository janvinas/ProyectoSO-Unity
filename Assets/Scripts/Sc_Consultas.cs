using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

public class Sc_Consultas : MonoBehaviour
{
    Socket server = PantallaPrincipal.server;
    ConcurrentQueue<string> queue = PantallaPrincipal.responseQueue;
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI textToDisplay;
    GameObject textBox;
    // Start is called before the first frame update
    void Start()
    {
        textBox = GameObject.Find("ConsultaNombre");
    }

    // Update is called once per frame
    void Update()
    {
        while(queue.Count > 0)
        {
            queue.TryDequeue(out string respuesta);
            EjecutarRespuesta(respuesta);
        }
    }
    private void EjecutarRespuesta(string respuesta)
    {
        Debug.Log("Mensaje recibido: " +  respuesta);
        string[] trozos = respuesta.Split(new[] { '/' }, 2);
        int codigo = Convert.ToInt32(trozos[0]);
        string mensaje = trozos[1];

        switch (codigo)
        {
            case 20:
                EscribirJugadores(mensaje);
                break;
            case 21:
                EscribirResultados(mensaje);
                break;
        }
    }

    

    public void ConsultarJugadores()
    {
        string name = PantallaPrincipal.usuario;
        string mensaje = $"20/{name}";
        byte[] bytes = Encoding.ASCII.GetBytes(mensaje);
        server.Send(bytes);
        titulo.text = "Jugadores con los que has jugado:";
    }
    public void ConsultaResultados()
    {
        string name = PantallaPrincipal.usuario;
        string jugadorAConsultar=textBox.GetComponent<TMP_InputField>().text;
        if(jugadorAConsultar!="")
        {
            string mensaje = $"20/{name}/{jugadorAConsultar}";
            byte[] bytes = Encoding.ASCII.GetBytes(mensaje);
            server.Send(bytes);
            titulo.text = "Resultados contra " + jugadorAConsultar + ":";
        }
    }
    private void EscribirJugadores(string mensaje)
    {

    }
    private void EscribirResultados(string mensaje)
    {
        
    }
}
