using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    [SerializeField] Button _startGame;
    [SerializeField] Button _salirGame;
    void Start()
    {
        _startGame.onClick.AddListener(StartNewGame);
        _salirGame.onClick.AddListener(SalirJuego);
    }
    
    private void StartNewGame()
    {
        ScenesManager.Instance.LoadNewGame();
    }
    private void SalirJuego()
    {
        Application.Quit();
    }

}
