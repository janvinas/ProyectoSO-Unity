using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    public static Color colorPrincipal = Color.white;
    private void Awake()
    {
        Instance = this;
    }
    public enum Scene
    {
        Main,
        Game1,
        Game2,
        Maps,
        Personalizar,
        PantallaPrincipal

    }
    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public void LoadNewGame()
    {
        SceneManager.LoadScene(Scene.Maps.ToString());
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadMain()
    {
        SceneManager.LoadScene(Scene.PantallaPrincipal.ToString());
    }
    public void LoadMap1()
    {
        SceneManager.LoadScene(Scene.Game1.ToString());
    }
    public void LoadMap2()
    {
        SceneManager.LoadScene(Scene.Game2.ToString());
    }
    public void LoadPerso()
    {
        SceneManager.LoadScene(Scene.Personalizar.ToString());
    }
}
