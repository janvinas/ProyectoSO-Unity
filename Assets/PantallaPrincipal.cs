using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PantallaPrincipal : MonoBehaviour
{
    private class Conectado
    {
        string nombre;
        bool invitado;
    }

    public string usuario = null;

    private UiElements uiElements;
    [SerializeField] Login login;
    [SerializeField] Register register;

    public Socket server;

    private ConcurrentQueue<string> responseQueue = new ConcurrentQueue<string>();
    private Thread threadServidor;

    private List<Conectado> listaConectados;


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
        Debug.Log("Procesando respuesta " +  respuesta);
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

        }
    }

    private void ActualizarListaConectados(string mensaje)
    {
        string[] trozos = mensaje.Split('/');
        int usuariosConectados = Convert.ToInt32(trozos[0]);
        uiElements.mainPanelNumeroConectados.text = "Conectados: " + usuariosConectados + "\n";
        int i = 1;

        while(i < trozos.Length)
        {
            i++;
        }

        // elimina todos los descendientes
        foreach(Transform t in uiElements.mainPanelListaConectados.transform.Find("Viewport").Find("Content"))
        {
            if(t.gameObject.name != "OnlineTag")
            {
                Destroy(t.gameObject);
            }
        }
        
        //añade todos los conectados a la lista de descendientes
        foreach (Conectado c in listaConectados)
        {
            GameObject text = new GameObject("username_" + trozos[i]);
            TextMeshProUGUI textMeshPro = text.AddComponent<TextMeshProUGUI>();
            Button button = text.AddComponent<Button>();

            textMeshPro.text = trozos[i];
            textMeshPro.fontStyle = FontStyles.Normal;
            textMeshPro.fontSize = 16;
            textMeshPro.color = new Color(0.8f, 0.8f, 0.8f);
            text.transform.SetParent(uiElements.mainPanelListaConectados.transform.Find("Viewport").Find("Content"), false);
            text.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -15 - 20 * i);
            text.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            text.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(175, 20);
        }

    }

    public void ConectadoClick(TextMeshProUGUI text)
    {

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
