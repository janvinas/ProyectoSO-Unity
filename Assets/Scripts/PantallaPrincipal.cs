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
using UnityEngine.SceneManagement;
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

    public static string usuario = null;
    public static int idPartida = -1;

    private UiElements uiElements;
    [SerializeField] Login login;
    [SerializeField] Register register;
    [SerializeField] TMP_FontAsset font;

    public static Socket server;
    public static ConcurrentQueue<string> responseQueue = new ConcurrentQueue<string>();
    public static Thread threadServidor;

    private List<Conectado> listaConectados = new List<Conectado>();

    private static void AtenderServidor()
    {
        while(true)
        {
            if (server == null || !server.Connected)
            {
                return;
            }
            else
            {
                byte[] bytes = new byte[512];
                try
                {
                    server.Receive(bytes);
                    string respuesta = Encoding.ASCII.GetString(bytes).Split('\0')[0];
                    string[] trozos = respuesta.Split('\n');
                    foreach(string trozo in trozos)
                    {
                        if(trozo.Length != 0)
                        {
                            responseQueue.Enqueue(trozo);
                        }
                    }
                }
                catch(Exception e )
                {
                    Debug.Log(e.Message);
                    return;
                }
                
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
            case 8:
                idPartida = Convert.ToInt32(mensaje);
                uiElements.mainPanelInvitarJugadores.interactable = false;
                break;
            case 9:
                MostrarInvitacion(mensaje);
                break;
            case 11:
                InvitacionAceptadaOtro(mensaje);
                break;
            case 13:
                Chat c = uiElements.mainPanelChat.GetComponent<Chat>();
                c.printMessage(mensaje.Split("/")[0] + ": " + mensaje.Split("/")[1], uiElements.accentColor);
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
            textMeshPro.color = uiElements.accentColor;
            textMeshPro.font = font;
            text.transform.SetParent(uiElements.mainPanelListaConectados.transform.Find("Viewport").Find("Content"), false);
            uiElements.mainPanelListaConectados.transform.Find("Viewport").Find("Content").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 55 + 20*j);
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

        text.color = conectado.invitado ? uiElements.bgColor : uiElements.accentColor;
    }

    public void InvitarSeleccionados()
    {
        //obté una llista dels connectats que han estat convidats
        List<Conectado> invitados = listaConectados.FindAll(c => c.invitado);
        if(invitados.Count == 0) return;

        string message = "8/" + invitados.Count();
        foreach(Conectado invitado in invitados)
        {
            message += "/" + invitado.nombre;
        }

        byte[] msg = Encoding.ASCII.GetBytes(message);
        server.Send(msg);

        uiElements.mainPanelChat.SetActive(true);
        uiElements.mainPanelIniciarPartida.SetActive(true);

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
        uiElements.mainPanelChatInput.SetActive(true);
        uiElements.mainPanelChat.SetActive(true);
        uiElements.mainPanelInvitarJugadores.interactable = !aceptada;
    }

    private void InvitacionAceptadaOtro(string message)
    {
        string[] trozos = message.Split('/');
        uiElements.mainPanelChat.GetComponent<Chat>().printMessage(trozos[1] + " ha entrado a la partida!", new Color(0.7f, 0.7f, 0.7f));
    }

    public void Start()
    {
        uiElements = this.GetComponent<UiElements>();

        //si el servidor ja està connectat:
        if(server != null && server.Connected)
        {
            uiElements.mainPanelConnectButton.interactable = false;
            if(usuario != null)
            {
                //sessió ja iniciada:
                uiElements.mainPanelUserInfo.gameObject.SetActive(true);
                uiElements.mainPanelListaConectados.gameObject.SetActive(true);
                uiElements.mainPanelInvitarJugadores.gameObject.SetActive(true);

                //uiElements.mainPanelInvitarJugadores.interactable = false;
                uiElements.mainPanelChat.SetActive(idPartida != -1);
            }
            else
            {
                uiElements.mainPanelLoginButton.interactable = false;
                uiElements.mainPanelRegisterButton.interactable = false;
            }
        }
    }

    private void Update()
    {
        while(responseQueue.Count > 0)
        {
            responseQueue.TryDequeue(out string respuesta);
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

        int port = 50065;
        IPEndPoint ipep;

        string[] trozos = uiElements.mainPanelServerAddress.GetComponent<TMP_InputField>().text.Split(":");
        if(trozos.Length == 0) {
            uiElements.mainPanelMessageBox.text = "Proporciona una dirección de servidor!";
            return;
        }else if(trozos.Length == 2)
        {
            try
            {
                port = Convert.ToInt32(trozos[1]);
            }
            catch(Exception)
            {
                uiElements.mainPanelMessageBox.text = "Formato de puerto incorrecto.";
                return;
            }
        }

        try
        {
            IPAddress direc = Dns.GetHostAddresses(trozos[0])[0];
            ipep = new IPEndPoint(direc, port);
        }
        catch (Exception) {
            uiElements.mainPanelMessageBox.text = "Error resolviendo la dirección";
            return;
        }
        


        //Creamos el socket 
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            server.Connect(ipep);//Intentamos conectar el socket
        }
        catch
        {
            uiElements.mainPanelConnectionIndicator.color = Color.red;
            uiElements.mainPanelMessageBox.text = "No se ha podido conectar con el servidor";
            return;
        }

        uiElements.mainPanelConnectionIndicator.color = Color.green;
        uiElements.mainPanelMessageBox.text = "Conectado con el servidor";

        uiElements.mainPanelConnectButton.interactable = false;
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
            return;
        }

        string mensaje = "0/";
        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        // Se terminó el servicio. 
        // Nos desconectamos
        uiElements.mainPanel.color = Color.gray;
        if(threadServidor.IsAlive) threadServidor.Abort();
        server.Shutdown(SocketShutdown.Both);
        server.Close();

        
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

    public void IniciarPartida()
    {
        SceneManager.LoadScene("Maps");
    }

    public void CerrarPantalla()
    {

    }

}
