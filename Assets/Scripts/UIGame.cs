
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    private Button _map1Game;
    private Button _map3Game;
    [SerializeField] Button _map2Game;
    [SerializeField] Button _salirGame;
    private Button _personalizar;
    void Start()
    {
        GameObject boton1 = GameObject.Find("Mapa1");
        _map1Game = boton1.GetComponent<Button>();
        GameObject boton3 = GameObject.Find("Mapa3");
        _map3Game = boton3.GetComponent<Button>();
        GameObject boton2 = GameObject.Find("Personalizar");
        _personalizar = boton2.GetComponent<Button>();

        _map1Game.onClick.AddListener(Map1);
        _map2Game.onClick.AddListener(Map2);
        _map3Game.onClick.AddListener(Map3);
        _salirGame.onClick.AddListener(Return3);
        _personalizar.onClick.AddListener(Return4);
    }
    
    private void Map1()
    {
        SendStartingMessage("1");
        ScenesManager.Instance.LoadMap1();
    }
    private void Map2()
    {
        SendStartingMessage("2");
        ScenesManager.Instance.LoadMap2();
    }
    private void Map3()
    {
        SendStartingMessage("3");
        ScenesManager.Instance.LoadMap3();
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
