using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class PantallaPrincipal : MonoBehaviour
{
    private class Conectado
    {
        public string nombre;
        public bool invitado;

        public Conectado(string nombre, bool invitado)
        {
            this.nombre = nombre;
            this.invitado = invitado;
        }
    }

    public string usuario = null;
    public int idPartida = -1;

    private UiElements uiElements;
    [SerializeField] Login login;
    [SerializeField] Register register;

    public Socket server;

    private ConcurrentQueue<string> responseQueue = new ConcurrentQueue<string>();
    private Thread threadServidor;

    private List<Conectado> listaConectados = new List<Conectado>();


    int receiveBufferPosition = 0;
    byte[] receiveBuffer = new byte[1024];
    private void AtenderServidor()
    {
        while(true)
        {
            if (server == null || !server.Connected) return;

            server.Receive(receiveBuffer, receiveBufferPosition, 1, SocketFlags.None);

            if (receiveBuffer[receiveBufferPosition] == '\n')
            {
                string respuesta = Encoding.ASCII.GetString(receiveBuffer, 0, receiveBufferPosition).Split('\0')[0];
                responseQueue.Enqueue(respuesta);
                receiveBufferPosition = 0;
            }
            else
            {
                receiveBufferPosition++;
            } 
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
            case 1:
                login.OnServerResponse(codigo, mensaje);
                break;
            case 2:
                register.OnServerResponse(codigo, mensaje);
                break;
            case 3:
                login.OnServerResponse(codigo, mensaje);
                register.OnServerResponse(codigo, mensaje);
                break;
            case 7:
                ActualizarListaConectados(mensaje);
                break;
            case 9:
                MostrarInvitacion(mensaje);
                break;

        }
    }

    private void ActualizarListaConectados(string mensaje)
    {
        string[] trozos = mensaje.Split('/');
        int usuariosConectados = Convert.ToInt32(trozos[0]);
        uiElements.mainPanelNumeroConectados.text = "Conectados: " + usuariosConectados + "\n";

        //elimina los conectados que ya no lo están
        foreach(Conectado c in listaConectados.ToList())
        {
            if (!trozos.Contains(c.nombre))
            {
                listaConectados.Remove(c);
            }
        }

        //añade los conectados que faltaban
        for(int i = 1; i < trozos.Length; i++)
        {
            if (!listaConectados.Any(c => c.nombre == trozos[i]))
            {
                listaConectados.Add(new Conectado(trozos[i], false));
            }
        }

        // elimina todos los descendientes
        foreach(Transform t in uiElements.mainPanelListaConectados.transform.Find("Viewport").Find("Content"))
        {
            if(t.gameObject.name != "OnlineTag")
            {
                Destroy(t.gameObject);
            }
        }

        int j = 0;
        //añade todos los conectados a la lista de descendientes
        foreach (Conectado c in listaConectados)
        {
            GameObject text = new GameObject("username_" + c.nombre);
            TextMeshProUGUI textMeshPro = text.AddComponent<TextMeshProUGUI>();
            Button button = text.AddComponent<Button>();

            textMeshPro.text = c.nombre;
            textMeshPro.fontStyle = FontStyles.Normal;
            textMeshPro.fontSize = 16;
            textMeshPro.color = new Color(0.8f, 0.8f, 0.8f);
            text.transform.SetParent(uiElements.mainPanelListaConectados.transform.Find("Viewport").Find("Content"), false);
            text.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -35 - 20 * j);
            text.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            text.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(175, 20);
            button.onClick.AddListener( delegate { Conectado_OnClick(textMeshPro); } );
            j++;
        }

    }

    public void Conectado_OnClick(TextMeshProUGUI text)
    {
        if (text.text == usuario) return;

        Conectado conectado = listaConectados.Find(c => c.nombre == text.text);
        conectado.invitado = !conectado.invitado;

        text.color = conectado.invitado ? new Color(0f, 0.6f, 0f) : new Color(0.8f, 0.8f, 0.8f);
    }

    public void InvitarSeleccionados()
    {
        List<Conectado> invitados = listaConectados.FindAll(c => c.invitado == true);
        if(invitados.Count == 0) return;

        string message = "8/" + invitados.Count();
        foreach(Conectado invitado in invitados)
        {
            message += "/" + invitado.nombre;
        }

        byte[] msg = Encoding.ASCII.GetBytes(message);
        server.Send(msg);
    }

    private void MostrarInvitacion(string message)
    {
        string[] trozos = message.Split("/");
        idPartida = Convert.ToInt32(trozos[0]);

        uiElements.notificionInvitacionPanel.gameObject.SetActive(true);
        uiElements.notificionInvitacionPanel.transform.Find("Invitacion").GetComponent<TextMeshProUGUI>().text =
            trozos[1] + " le ha invitado a una partida! (ID partida: " + trozos[0] + ")";
    }
    
    public void CerrarInvitacion(bool aceptada)
    {
        string mensaje = "10/" + idPartida.ToString() + (aceptada ? "/1" : "/0");  
        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        uiElements.notificionInvitacionPanel.gameObject.SetActive(false);
    }

    public void Start()
    {
       uiElements = this.GetComponent<UiElements>();
    }

    private void Update()
    {
        while(responseQueue.Count > 0)
        {
            string respuesta;
            responseQueue.TryDequeue(out respuesta);
            EjecutarRespuesta(respuesta);
        }
    }

    private void OnApplicationQuit()
    {
        if (server != null && server.Connected)
        {
            string mensaje = "0/";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            threadServidor.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
    }

    public void ConectarServidor(){
        uiElements.mainPanelMessageBox.text = "Conectando con el servidor...";

        //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
        //al que deseamos conectarnos
        IPAddress direc = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipep = new IPEndPoint(direc, 9050);


        //Creamos el socket 
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            server.Connect(ipep);//Intentamos conectar el socket
        }
        catch
        {
            uiElements.mainPanel.color = Color.red;
            uiElements.mainPanelMessageBox.text = "No se ha podido conectar con el servidor";
            return;
        }

        uiElements.mainPanel.color = Color.green;
        uiElements.mainPanelMessageBox.text = "Conectado con el servidor";

        uiElements.mainPanelConnectButton.interactable = false;
        uiElements.mainPanelDisconnectButton.interactable = true;
        uiElements.mainPanelLoginButton.interactable = true;
        uiElements.mainPanelRegisterButton.interactable = true;

        ThreadStart ts = delegate { AtenderServidor(); };
        threadServidor = new Thread(ts);
        threadServidor.Start();
    }

    public void DesconectarServidor()
    {

        if (server == null || !server.Connected)
        {
            uiElements.mainPanelMessageBox.text = "No estabas conectado. Conéctate presionando \"Conectar\"";
            return;
        }

        string mensaje = "0/";
        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        // Se terminó el servicio. 
        // Nos desconectamos
        uiElements.mainPanel.color = Color.gray;
        threadServidor.Abort();
        server.Shutdown(SocketShutdown.Both);
        server.Close();

        usuario = null;
        uiElements.mainPanelListaConectados.gameObject.SetActive(false);
        uiElements.mainPanelUserInfo.gameObject.SetActive(false);
        uiElements.mainPanelInvitarJugadores.gameObject.SetActive(true);
        uiElements.mainPanelLoginButton.interactable = false;
        uiElements.mainPanelRegisterButton.interactable = false;
        uiElements.mainPanelMessageBox.text = "Desconectado. Conéctate presionando \"Conectar\"";
        uiElements.mainPanelConnectButton.interactable = true;
        uiElements.mainPanelDisconnectButton.interactable = false;
        
    }

    public void AbrirLogin()
    {
        uiElements.loginPanel.gameObject.SetActive(true);
        uiElements.mainPanel.GetComponent<CanvasGroup>().interactable = false;
    }

    public void AbrirRegister()
    {
        uiElements.registerPanel.gameObject.SetActive(true);
        uiElements.mainPanel.GetComponent<CanvasGroup>().interactable = false;
    }



}
