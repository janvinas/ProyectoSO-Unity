
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    private Button _map1Game;
    [SerializeField] Button _map2Game;
    [SerializeField] Button _salirGame;
    private Button _personalizar;
    void Start()
    {
        GameObject boton1 = GameObject.Find("Mapa1");
        _map1Game = boton1.GetComponent<Button>();
        GameObject boton2 = GameObject.Find("Personalizar");
        _personalizar = boton2.GetComponent<Button>();

        _map1Game.onClick.AddListener(Return);
        _map2Game.onClick.AddListener(Return2);
        _salirGame.onClick.AddListener(Return3);
        _personalizar.onClick.AddListener(Return4);
    }
    
    private void Return()
    {
        SendStartingMessage("1");
        ScenesManager.Instance.LoadMap1();
    }
    private void Return2()
    {
        SendStartingMessage("2");
        ScenesManager.Instance.LoadMap2();
    }
    private void Return3()
    {
        ScenesManager.Instance.LoadMain();
    }
    private void Return4()
    {
        ScenesManager.Instance.LoadPerso();
    }

    private void SendStartingMessage(string mapa)
    {
        string message = "15/" + PantallaPrincipal.idPartida + "/" + mapa;
        byte[] msg = Encoding.ASCII.GetBytes(message);

        if (PantallaPrincipal.server == null || !PantallaPrincipal.server.Connected) return;

        PantallaPrincipal.server.Send(msg);
    }

}
