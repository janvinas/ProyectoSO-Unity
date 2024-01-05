using UnityEngine;
using UnityEngine.UI;

public class UIPerso : MonoBehaviour
{
    [SerializeField] Button _salirGame;
    void Start()
    {
        _salirGame.onClick.AddListener(SalirJuego);
    }
    private void SalirJuego()
    {
        ScenesManager.Instance.LoadMain();
    }

}
