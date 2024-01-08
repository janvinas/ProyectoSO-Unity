using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{   

    private Socket server;

    private UiElements uiElements;

    public Canvas mainCanvas;
    public GameObject mainPanel;
    public TMP_InputField usuario;
    public TMP_InputField password;
    public TextMeshProUGUI mensaje;
    public Button registerButton;
    public Button loginButton;

    // Start is called before the first frame update
    void Start()
    {
        server = PantallaPrincipal.server;
        uiElements = mainCanvas.GetComponent<UiElements>();
        usuario.text = "";
        password.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        server = PantallaPrincipal.server;
    }

    public void SendLogin()
    {
        if (server == null || !server.Connected) {
            return;
        }

        if(usuario.text == "" || password.text == "")
        {
            return;
        }

        string mensaje = "1/" + usuario.text.Replace("/","") + "/" + password.text.Replace("/","") + "\n";
        // Enviamos al servidor el nombre tecleado
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);

    }

    public void OnServerResponse(int responseCode, string message)
    {
        if (!this.gameObject.activeSelf) return;

        if (responseCode == 1)
        {
            if (message == "1")
            {
                PantallaPrincipal.usuario = usuario.text.Replace("/","");

                uiElements.mainPanelUserInfo.gameObject.SetActive(true);
                uiElements.mainPanelUsername.text = usuario.text.Replace("/","");
                uiElements.mainPanelListaConectados.gameObject.SetActive(true);
                uiElements.mainPanelInvitarJugadores.gameObject.SetActive(true);
                uiElements.mainPanelLoginButton.interactable = false;
                uiElements.mainPanelRegisterButton.interactable = false;

                //pide la experiencia del usuario
                Debug.Log(PantallaPrincipal.usuario);
                string mensaje = "23/" + PantallaPrincipal.usuario + "\n";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ClosePanel();
            }
            else if (message == "0")
            {
                mensaje.text = "Usuario o contrase�a incorrectos";
                usuario.text = "";
                password.text = "";
            }
            else
            {
                mensaje.text = "Error de base de datos";
            }
        }
    }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
        mainPanel.GetComponent<CanvasGroup>().interactable = true;
    }
}
