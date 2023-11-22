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
        server = mainCanvas.GetComponent<PantallaPrincipal>().server;
        uiElements = mainCanvas.GetComponent<UiElements>();
        usuario.text = "";
        password.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
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

        string mensaje = "1/" + usuario.text + "/" + password.text;
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
                mainCanvas.GetComponent<PantallaPrincipal>().usuario = usuario.text;

                uiElements.mainPanelUserInfo.gameObject.SetActive(true);
                uiElements.mainPanelUsername.text = usuario.text;
                uiElements.mainPanelListaConectados.gameObject.SetActive(true);
                uiElements.mainPanelInvitarJugadores.gameObject.SetActive(true);
                loginButton.interactable = false;
                registerButton.interactable = false;

                ClosePanel();
            }
            else if (message == "0")
            {
                mensaje.text = "Usuario o contraseña incorrectos";
                usuario.text = "";
                password.text = "";
            }
            else
            {
                mensaje.text = "Error de base de datos";
            }
        }else if(responseCode == 3)
        {
            TMP_Text text = usuario.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TMP_Text>();
            Debug.Log(text.ToString());
            if (message == "1")
            {
                text.color = new Color(0, 0.8f, 0);
            }
            else
            {
                text.color = new Color(0.1960f, 0.1960f, 0.1960f);
            }
        }
    }

    public void OnUsernameChanged(TMP_InputField inputField)
    {
        if (server == null || !server.Connected || inputField.text == "")
        {
            return;
        }

        string mensaje = "3/" + inputField.text;
        // Enviamos al servidor el nombre tecleado
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
    }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
        mainPanel.GetComponent<CanvasGroup>().interactable = true;
    }
}
