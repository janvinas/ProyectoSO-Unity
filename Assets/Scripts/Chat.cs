using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{

    [SerializeField] UiElements uiElements;

    private Transform content;

    private const int verticalIncrement = 17;

    private int messageNumber = 0;
    private int lowerBoundary = -5;

    private Socket server = PantallaPrincipal.server;

    void Start()
    {
        content = this.gameObject.transform.Find("Viewport").Find("Content");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sendMessage()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        PantallaPrincipal p = uiElements.GetComponent<PantallaPrincipal>();
        if (server == null || !server.Connected || PantallaPrincipal.usuario == null || PantallaPrincipal.idPartida == -1) return;
        uiElements.mainPanelChatInput.GetComponent<TMP_InputField>().Select();
        uiElements.mainPanelChatInput.GetComponent<TMP_InputField>().ActivateInputField();
        string text = uiElements.mainPanelChatInput.GetComponent<TMP_InputField>().text.Replace("\n", "");
        if (text == "") return;

        string message = "12/" + PantallaPrincipal.idPartida + "/" + PantallaPrincipal.usuario + "/" + text;
        Debug.Log("Mensaje enviado " + message);
        byte[] msg = Encoding.ASCII.GetBytes(message);
        server.Send(msg);
        uiElements.mainPanelChatInput.GetComponent<TMP_InputField>().text = "";
    }

    public void printMessage(string message, Color color)
    {
        GameObject text = new GameObject("message" + messageNumber);
        TextMeshProUGUI textMeshPro = text.AddComponent<TextMeshProUGUI>();
        ContentSizeFitter textSizeFitter = text.AddComponent<ContentSizeFitter>();
        
        textMeshPro.text = message;
        textMeshPro.fontStyle = FontStyles.Normal;
        textMeshPro.fontSize = 12;
        textMeshPro.color = color;
        text.transform.SetParent(content, false);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, lowerBoundary);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 0);   
        textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        lowerBoundary -= (int)textMeshPro.preferredHeight;
        content.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1, - lowerBoundary + 10);
        this.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;

        messageNumber++;
    }
}
