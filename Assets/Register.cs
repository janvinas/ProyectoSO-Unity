using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Register : MonoBehaviour
{
    public Canvas mainCanvas;
    public GameObject mainPanel;
    public TMP_InputField usuario;
    public TMP_InputField email;
    public TMP_InputField genero;
    public TMP_InputField password;
    public TMP_InputField repetirPassword;
    public TextMeshProUGUI mensaje;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnServerResponse(int responseCode,  string message)
    {
        if (!this.gameObject.activeSelf) return;

        if (responseCode == 2)
        {
            if (message == "1")
            {
                ClosePanel();
            }
            else if (message == "0")
            {
                mensaje.text = "El usuario ya existe!";
                usuario.text = "";
            }
            else
            {
                mensaje.text = "Error de base de datos";
            }
        }
        else if (responseCode == 3)
        {
            TMP_Text text = usuario.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TMP_Text>();
            if (message == "1")
            {
                text.color = new Color(0.8f, 0, 0);
            }
            else
            {
                text.color = new Color(0.1960f, 0.1960f, 0.1960f);
            }
        }
    }

    public void RegisterClick()
    {
        if (usuario.text == "" || email.text == "" || genero.text == "" || password.text == "" || repetirPassword.text == "")
        {
            mensaje.text = "Debes rellenar todos los campos";
            return;
        }
        else if (password.text != repetirPassword.text)
        {
            mensaje.text = "Las contraseñas no coinciden";
            return;
        }
        else if (genero.text != "M" && genero.text != "H")
        {
            mensaje.text = "Valor incorrecto introducido en 'Género'";
            return;
        }

        Socket server = mainCanvas.GetComponent<PantallaPrincipal>().server;
        string message = "2/" + usuario.text + "/" + password.text;
        Debug.Log("enviando respuesta" + message);
        // Enviamos al servidor el nombre tecleado
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
        server.Send(msg);

    }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
        mainPanel.GetComponent<CanvasGroup>().interactable = true;
    }

    public void OnUsernameChanged(TMP_InputField inputField)
    {
        Socket server = mainCanvas.GetComponent<PantallaPrincipal>().server;
        if (server == null || !server.Connected || inputField.text == "")
        {
            return;
        }

        string mensaje = "3/" + inputField.text;
        // Enviamos al servidor el nombre tecleado
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        server.Send(msg);
    }
}
