using UnityEngine;
using UnityEngine.UI;

public class Sc_Car : MonoBehaviour
{
    [SerializeField] Text _temporizador;
    [SerializeField] Text _temporizadorMax;
    [SerializeField] GameObject ultimaVelta;
    [SerializeField] GameObject finCarrera;
    [SerializeField] Sc_MenuBehavior sc_MenuBehavior;
    public float Velocidad;
    private float mHorizontal,mVertical;
    private Rigidbody2D rigid;
    private bool meta,mostrado;
    private float currentTime = 0f;
    public int vueltasMax=3,vueltas;
    private float timerMax=0f;
    private Color colorPrincipal;
    void Start()
    {
        Velocidad = 2f;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        meta=false;
        mostrado=false;
        vueltas=vueltasMax;
        colorPrincipal = ScenesManager.colorPrincipal;
        gameObject.GetComponent<Renderer>().material.color = colorPrincipal;
    }
    void Update()
    {
        mHorizontal = Input.GetAxis("Horizontal");
        mVertical = Input.GetAxis("Vertical");        
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if(sc_MenuBehavior.acabado && vueltas!=0)
        {
            conteo();
        }
        if(vueltas==1)
        {
            if(!mostrado)
            {
                ultimaVelta.SetActive(true);
                mostrado=true;
            }
            else
                Invoke("quitarUtlimaVuelta",3f);
        }
        if(vueltas==0)
        {
            Velocidad=0f;
            meta=true;
            finCarrera.SetActive(true);
        }
    }
    void FixedUpdate()
    {
        if(Mathf.Abs(mHorizontal) > 0.1f)
        {
            rigid.AddForce(new Vector2(mHorizontal*Velocidad, 0f),ForceMode2D.Impulse);
        }
        if(Mathf.Abs(mVertical) > 0.1f)
        {
            rigid.AddForce(new Vector2(0f, mVertical*Velocidad),ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Meta"))
        {
            meta=true;
            vueltas-=1;   
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Meta") )
        {
            meta=false;
        }
    }
    private void conteo()
    {
        if(meta)
        {
            if(vueltas==vueltasMax-1)
            {
                timerMax = currentTime;
                _temporizadorMax.text = timerMax.ToString("0:00.000");
            }
            else if(currentTime-timerMax<timerMax)
            {
                timerMax = currentTime-timerMax;
                _temporizadorMax.text = timerMax.ToString("0:00.000");
            }
        }
        currentTime+=Time.deltaTime;
        _temporizador.text = currentTime.ToString("0:00.000");
    }
    void quitarUtlimaVuelta()
    {
        ultimaVelta.SetActive(false);
    }
}
