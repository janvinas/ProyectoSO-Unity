using UnityEngine;
using UnityEngine.UI;

public class Sc_movimiento : MonoBehaviour
{

    //[SerializeField] GameObject nombre;
    // Start is called before the first frame update
    private Vector2 screenBounds;
    private Vector2 mida;

    private Rigidbody2D rb;

    void Start()
    {
       screenBounds=Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,Camera.main.transform.position.z));
       mida = GetComponent<Renderer>().bounds.size;

       rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x,-screenBounds.x + mida.x/2,screenBounds.x-mida.x/2),
        Mathf.Clamp(transform.position.y,-screenBounds.y+mida.y/2,screenBounds.y-mida.y/2),transform.position.z);

        if(rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            //nombre.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
    }
    
}
