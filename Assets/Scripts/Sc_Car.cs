using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System.Text;

public class Sc_Car : MonoBehaviour
{
    [SerializeField] Text _temporizador;
    [SerializeField] Text _temporizadorMax;
    [SerializeField] GameObject ultimaVelta;
    [SerializeField] GameObject finCarrera;
    [SerializeField] Sc_MenuBehavior sc_MenuBehavior;
    Socket server = PantallaPrincipal.server;
    public float Velocidad;
    public float nitro;
    private float mHorizontal,mVertical;
    private Rigidbody2D rigid;
    private bool meta,mostrado;
    private float currentTime = 0f;
    public int vueltasMax=3,vueltas;
    private float timerMax=0f;
    private Color colorPrincipal;
    public float tiempoDuracionNitro = 0.5f;
    public int contNitro = 0;
    public TextMeshProUGUI texto;
    private Text NombreUsuario;
    private bool enviado;
    
    void Start()
    {
        texto.text = "0";
        Velocidad = 0.7f;
        nitro = 2f;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        meta=false;
        mostrado=false;
        vueltas=vueltasMax;
        colorPrincipal = ScenesManager.colorPrincipal;
        gameObject.GetComponent<Renderer>().material.color = colorPrincipal;
        NombreUsuario = transform.Find("Name").Find("Name").GetComponent<Text>();
        NombreUsuario.text = PantallaPrincipal.usuario;
        enviado=false;
    }
    void Update()
    {
        mHorizontal = Input.GetAxis("Horizontal");
        mVertical = Input.GetAxis("Vertical");
        if (sc_MenuBehavior.acabado && vueltas!=0)
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
            if(!enviado)
                enviarTiempos();
                enviado=true;
        }
        if (Input.GetKeyDown(KeyCode.N))// Verifica si la tecla "N" est� siendo presionada, y llama a la funcion activarNitro
        {
            if (contNitro>0)
            {
                activaNitro();
                contNitro = contNitro - 1;
                texto.text = contNitro.ToString();
            }
        }
    }
    void FixedUpdate()
    {
        if(Mathf.Abs(mHorizontal) > 0.1f)
        {
            rigid.AddForce(new Vector2(3*mHorizontal*Velocidad, 0f),ForceMode2D.Impulse);
        }
        if(Mathf.Abs(mVertical) > 0.1f)
        {
            rigid.AddForce(new Vector2(0f, 3*mVertical*Velocidad),ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Meta"))
        {
            meta=true;
            vueltas-=1;   
        }
        if (collision.transform.CompareTag("Nitro"))
        {
            contNitro += 1;
            texto.text = contNitro.ToString();
        }
        if(collision.gameObject.CompareTag("Pista"))
        {
            rigid.drag = 2f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Meta") )
        {
            meta=false;
        }
        if(collision.gameObject.CompareTag("Pista"))
        {
            rigid.drag = 30f;
        }
    }
    private void conteo()
    {
        if(meta)
        {
            if(vueltas==vueltasMax-1)
            {
                timerMax = currentTime;
                int _minutos = Mathf.FloorToInt(timerMax / 60);
                int _segundos = Mathf.FloorToInt(timerMax % 60);
                float _milisegundos = (timerMax % 1) * 1000;

                _temporizadorMax.text = string.Format("{0:00}:{1:00}:{2:000}", _minutos, _segundos, _milisegundos);
            }
            else if(currentTime-timerMax<timerMax)
            {
                timerMax = currentTime-timerMax;
                _temporizadorMax.text = timerMax.ToString("0:00.000");
            }
        }
        currentTime+=Time.deltaTime;

        int minutos = Mathf.FloorToInt(currentTime / 60);
        int segundos = Mathf.FloorToInt(currentTime % 60);
        float milisegundos = (currentTime % 1) * 1000;

        _temporizador.text = string.Format("{0:00}:{1:00}:{2:000}", minutos, segundos, milisegundos);


    }
    void quitarUtlimaVuelta()
    {
        ultimaVelta.SetActive(false);
    }
    void VelocidadNormal()
    {
        //Vuelve a la velocidad sin nitro
        Velocidad = Velocidad / nitro;
    }
    void activaNitro()
    {
        Velocidad = nitro * Velocidad;
        Invoke("VelocidadNormal", tiempoDuracionNitro);  // Invoca la funci�n "VelocidadNormal" despu�s del tiempo especificado
    }
    void enviarTiempos()
    {
        string name = PantallaPrincipal.usuario;
        int idPartida = PantallaPrincipal.idPartida;
        string mensaje = $"17/{idPartida}/{name}/{currentTime}";
        byte[] bytes = Encoding.ASCII.GetBytes(mensaje);
        server.Send(bytes);
    }
}
