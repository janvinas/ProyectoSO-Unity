using UnityEngine;

public class Sc_movimiento : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 screenBounds;
    private Vector2 mida;
    void Start()
    {
       screenBounds=Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,Camera.main.transform.position.z));
       mida = GetComponent<Renderer>().bounds.size;
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x,-screenBounds.x + mida.x/2,screenBounds.x-mida.x/2),
        Mathf.Clamp(transform.position.y,-screenBounds.y+mida.y/2,screenBounds.y-mida.y/2),transform.position.z);
    }
    
}
