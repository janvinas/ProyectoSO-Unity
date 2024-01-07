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
    public GameObject dd1,mm1,aaaa1,dd2,mm2,aaaa2;
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
            case 22:
                JuegosEnTiempo(mensaje);
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
        textToDisplay.text = "";
    }
    public void ConsultaResultados()
    {
        string name = PantallaPrincipal.usuario;
        string jugadorAConsultar=textBox.GetComponent<TMP_InputField>().text;
        if(jugadorAConsultar!="")
        {
            string mensaje = $"21/{name}/{jugadorAConsultar}";
            byte[] bytes = Encoding.ASCII.GetBytes(mensaje);
            server.Send(bytes);
            titulo.text = "Resultados contra " + jugadorAConsultar + ":";
            textToDisplay.text = "";
        }
    }
    public void ConsultarDias()
    {
        string dia1=dd1.GetComponent<TMP_InputField>().text;
        string mes1=mm1.GetComponent<TMP_InputField>().text;
        string año1=aaaa1.GetComponent<TMP_InputField>().text;
        string dia2=dd2.GetComponent<TMP_InputField>().text;
        string mes2=mm2.GetComponent<TMP_InputField>().text;
        string año2=aaaa2.GetComponent<TMP_InputField>().text;
        if(dia1!="" && dia2!="" && mes1!="" && mes2!="" && año1!="" && año2!="")
        {
            string mensaje = $"22/{PantallaPrincipal.usuario}/{año1}-{mes1}-{dia1}/{año2}-{mes2}-{dia2}";
            byte[] bytes = Encoding.ASCII.GetBytes(mensaje);
            server.Send(bytes);
            titulo.text = "Lista de partidas jugadas en un tiempo:";
            textToDisplay.text = "";
        }
    }
    private void EscribirJugadores(string mensaje)
    {
        if(mensaje=="0")
        {
            textToDisplay.text = "No has jugado contra nadie";
        }
        else
        {
            string[] trozos = mensaje.Split('/');
            int i = 0;
            while (i < trozos.Length)
            {
                textToDisplay.text += " - " + trozos[i] + "\n";
                i++;
            }
        }
        
    }
    private void EscribirResultados(string mensaje)
    {
        if(mensaje=="0")
        {
            textToDisplay.text = "No has jugado contra este jugador";
        }
        else
        {
            string[] trozos = mensaje.Split('/');
            int i = 0;
            int IDpartida=0;
            int comp=0;
            while (i < trozos.Length)
            {
                if(comp!=Convert.ToInt32(trozos[i]))
                {
                    textToDisplay.text += "----------------------------------------- \n";
                    IDpartida++;
                }
                textToDisplay.text += IDpartida.ToString() + ". " + trozos[i+3] + "º "+ trozos[i+1] + " " + trozos[i+2]  + "\n";
                comp=Convert.ToInt32(trozos[i]);
                i+=4;
                
            }
        }
    }
    private void JuegosEnTiempo(string mensaje)
    {
        if(mensaje=="0")
        {
            textToDisplay.text = "No has jugado entre esos dias";
        }
        else
        {
            string[] trozos = mensaje.Split('/');
            int i = 0;
            while (i < trozos.Length)
            {
                textToDisplay.text += trozos[i] + ". [" + trozos[i+1] + "] " + trozos[i+2] + "s " + trozos[i+3] + "º\n";
                i+=4;
            }
        }
    }

}
