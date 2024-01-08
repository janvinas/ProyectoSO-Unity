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
    private Button _personalizar;
    public static string usuario = null;
    public static int idPartida = -1;
    public static bool host=false;
    public static bool activo = false;
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
            case 16:
                IniciarPartida(mensaje);
                break;
            case 23:
                ActualizarXP(mensaje);
                break;
        }
    }

    private void ActualizarXP(string mensaje)
    {
        if (mensaje == "-1") return;
        uiElements.mainPanelExperience.text = mensaje + "xp";
    }

    private void ActualizarListaConectados(string mensaje)
    {
        string[] trozos = mensaje.Split('/');
        int usuariosConectados = Convert.ToInt32(trozos[0]);
        uiElements.mainPanelNumeroConectados.text = "Conectados: " + usuariosConectados + "\n";

        //elimina los conectados que ya no lo est�n
        foreach(Conectado c in listaConectados.ToList())
        {
            if (!trozos.Contains(c.nombre))
            {
                listaConectados.Remove(c);
            }
        }

        //a�ade los conectados que faltaban
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
        //a�ade todos los conectados a la lista de descendientes
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
        //obt� una llista dels connectats que han estat convidats
        List<Conectado> invitados = listaConectados.FindAll(c => c.invitado);
        if(invitados.Count == 0) return;

        string message = "8/" + invitados.Count();
        foreach(Conectado invitado in invitados)
        {
            message += "/" + invitado.nombre;
        }
        message += "\n";
        Debug.Log(message);
        byte[] msg = Encoding.ASCII.GetBytes(message);
        server.Send(msg);
        host=true;
        uiElements.mainPanelChat.SetActive(true);
        uiElements.mainPanelIniciarPartida.SetActive(true);
        uiElements.mainPanelPracticarButton.interactable = false;

    }

    private void MostrarInvitacion(string message)
    {
        if(idPartida!=-1) return;
        string[] trozos = message.Split("/");
        idPartida = Convert.ToInt32(trozos[0]);

        uiElements.notificionInvitacionPanel.gameObject.SetActive(true);
        uiElements.notificionInvitacionPanel.transform.Find("Invitacion").GetComponent<TextMeshProUGUI>().text =
            trozos[1] + " le ha invitado a una partida! (ID partida: " + trozos[0] + ")";
    }
    
    public void CerrarInvitacion(bool aceptada)
    {
        string mensaje = "10/" + idPartida.ToString() + (aceptada ? "/1" : "/0") + "\n";  
        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);

        uiElements.notificionInvitacionPanel.gameObject.SetActive(false);
        uiElements.mainPanelChat.SetActive(aceptada);
        uiElements.mainPanelInvitarJugadores.interactable = !aceptada;
        uiElements.mainPanelPracticarButton.interactable = false;

    }

    private void InvitacionAceptadaOtro(string message)
    {
        string[] trozos = message.Split('/');
        uiElements.mainPanelChat.GetComponent<Chat>().printMessage(trozos[1] + " ha entrado a la partida!", new Color(0.7f, 0.7f, 0.7f));
    }

    public void Start()
    {
        uiElements = this.GetComponent<UiElements>();
        uiElements.mainPanelConnectionIndicator.color = Color.white;
        activo=false;
        //si el servidor ja est� connectat:
        if(server != null && server.Connected)
        {
            uiElements.mainPanelConnectButton.interactable = false;
            uiElements.mainPanelConnectionIndicator.color = Color.green;
            uiElements.mainPanelMessageBox.text = "";
            uiElements.mainPanelInvitarJugadores.interactable = (idPartida == -1);
            if (usuario != null)
            {
                //sessi� ja iniciada:
                uiElements.mainPanelUserInfo.gameObject.SetActive(true);
                uiElements.mainPanelListaConectados.gameObject.SetActive(true);
                uiElements.mainPanelUsername.text = usuario;
                uiElements.mainPanelDesconectarButton.interactable = true;
                uiElements.mainPanelLoginButton.interactable = false;
                uiElements.mainPanelRegisterButton.interactable = false;
                if(host)
                    uiElements.mainPanelIniciarPartida.SetActive(true);
                //uiElements.mainPanelInvitarJugadores.interactable = false;
                uiElements.mainPanelChat.SetActive(idPartida != -1);
                uiElements.mainPanelPracticarButton.interactable = idPartida == -1;

                string mensaje = "7/\n";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);


                mensaje = "23/" + PantallaPrincipal.usuario + "\n";
                msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                uiElements.mainPanelLoginButton.interactable = true;
                uiElements.mainPanelRegisterButton.interactable = true;
            }
        }
        else
        {
            uiElements.mainPanelMessageBox.text = "Presiona \"Conectar\" para conectarte al servidor.";
        }
        GameObject boton2 = GameObject.Find("Personalizar");
        _personalizar = boton2.GetComponent<Button>();
        _personalizar.onClick.AddListener(CargarPerso);
    }

    private void Update()
    {
        while(responseQueue.Count > 0)
        {
            responseQueue.TryDequeue(out string respuesta);
            EjecutarRespuesta(respuesta);
        }

    }
    private void CargarPerso()
    {
        ScenesManager.Instance.LoadPerso();
    }
    private void OnApplicationQuit()
    {
        if (server != null && server.Connected)
        {
            string mensaje = "0/\n";
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

        string[] trozos = uiElements.mainPanelServerAddress.GetComponent<TMP_InputField>().text.Replace("/","").Split(":");
        if(trozos.Length == 0) {
            uiElements.mainPanelMessageBox.text = "Proporciona una direcci�n de servidor!";
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
            uiElements.mainPanelMessageBox.text = "Error resolviendo la direcci�n";
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
        uiElements.mainPanelDesconectarButton.interactable = true;
    }

    public void DesconectarServidor()
    {
        
        //poner todas las variables en null.



        if (server == null || !server.Connected)
        {
            return;
        }

        string mensaje = "0/\n";
        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
        // Se termin� el servicio. 
        // Nos desconectamos
        if(threadServidor.IsAlive) threadServidor.Abort();
        server.Shutdown(SocketShutdown.Both);
        server.Close();
        uiElements.mainPanelPracticarButton.interactable = true;
        activo=false;
        uiElements.mainPanelDesconectarButton.gameObject.SetActive(false);
        uiElements.mainPanelEliminarButton.gameObject.SetActive(false);
        uiElements.mainPanelConsultasButton.gameObject.SetActive(false);


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

    public void SeleccionarMapa()
    {
        SceneManager.LoadScene("Maps");
    }

    public void IniciarPartida(string mensaje)
    {
        switch (mensaje)
        {
            case "1":
                ScenesManager.Instance.LoadMap1();
                break;
            case "2":
                ScenesManager.Instance.LoadMap2();
                break;
            case "3":
                ScenesManager.Instance.LoadMap3();
                break;
        }
    }

    public void CerrarPantalla()
    {
        DesconectarServidor();
        Application.Quit();
    }

    public void Desconectar_OnClick()
    {

        DesconectarServidor();
        usuario=null;
        uiElements.mainPanelDesconectarButton.interactable = false;
        uiElements.mainPanelConnectButton.interactable = true;
        uiElements.mainPanelRegisterButton.interactable = false;
        uiElements.mainPanelLoginButton.interactable = false;
        uiElements.mainPanelChat.gameObject.SetActive(false);
        uiElements.mainPanelListaConectados.gameObject.SetActive(false);
        uiElements.mainPanelIniciarPartida.SetActive(false);
        uiElements.mainPanelConnectionIndicator.color = Color.white;
        uiElements.mainPanelUserInfo.gameObject.SetActive(false);
        uiElements.mainPanelInvitarJugadores.interactable = false;
    }
    public void EliminarUsuario_OnClick()
    {
        string mensaje = "24/" + uiElements.mainPanelUsername.text + "\n";  
        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);

        Desconectar_OnClick();
    }
    public void MostrarMasOpciones()
    {
        if(!activo)
        {
            uiElements.mainPanelDesconectarButton.gameObject.SetActive(true);
            uiElements.mainPanelEliminarButton.gameObject.SetActive(true);
            uiElements.mainPanelConsultasButton.gameObject.SetActive(true);
            activo=true;
        }
        else if(activo)
        {
            uiElements.mainPanelDesconectarButton.gameObject.SetActive(false);
            uiElements.mainPanelEliminarButton.gameObject.SetActive(false);
            uiElements.mainPanelConsultasButton.gameObject.SetActive(false);
            activo=false;
        }
    }
    public void ConsultarParametros_OnClick()
    {
        ScenesManager.Instance.LoadConsultas();
    }
    public string nombreEscena;
    public void CargarEscena_OnClick()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
