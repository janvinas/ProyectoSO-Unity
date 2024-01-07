using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // Busca el objeto ra�z y aplica DontDestroyOnLoad
            GameObject root = GameObject.Find("SoundManagerRoot");

            if (root != null)
            {
                DontDestroyOnLoad(root);
            }
            else
            {
                Debug.LogError("No se encontr� el objeto ra�z para gestionar el sonido.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
