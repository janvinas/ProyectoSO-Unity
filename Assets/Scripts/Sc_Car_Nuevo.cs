using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Sc_Car_Nuevo : MonoBehaviour
{
    [SerializeField] Text _temporizador;
    [SerializeField] Text _temporizadorMax;
    [SerializeField] GameObject ultimaVelta;
    [SerializeField] GameObject finCarrera;
    [SerializeField] Sc_MenuBehavior sc_MenuBehavior;
    private Rigidbody2D rigid;
    private bool meta,mostrado;
    private float currentTime = 0f;
    public int vueltasMax=3,vueltas;
    private float timerMax=0f;
    private Color colorPrincipal;

    //Ajustes Coche
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30f;
    public float turnFactor = 3.5f;
    public float maxSpeed=20f;
    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;
    float velocityVsUp = 0;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        meta=false;
        mostrado=false;
        vueltas=vueltasMax;
        //colorPrincipal = ScenesManager.colorPrincipal;
        gameObject.GetComponent<Renderer>().material.color = colorPrincipal;
    }
    void Update()
    {
        /*mHorizontal = Input.GetAxis("Horizontal");
        mVertical = Input.GetAxis("Vertical");*/
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
            meta=true;
            finCarrera.SetActive(true);
        }
    }
    void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
        /*if(Mathf.Abs(mHorizontal) > 0.1f)
        {
            rigid.AddForce(new Vector2(mHorizontal*Velocidad, 0f),ForceMode2D.Impulse);
        }
        if(Mathf.Abs(mVertical) > 0.1f)
        {
            rigid.AddForce(new Vector2(0f, mVertical*Velocidad),ForceMode2D.Impulse);
        }*/
    }
    private void ApplyEngineForce()
    {
        velocityVsUp = Vector2.Dot(transform.up,rigid.velocity);
        if(velocityVsUp > maxSpeed && accelerationInput > 0)
            return;
        if(velocityVsUp < -maxSpeed*0.5f && accelerationInput<0)
            return;
        if(rigid.velocity.sqrMagnitude>maxSpeed*maxSpeed && accelerationInput>0)
            return;
        if(accelerationInput==0)
            rigid.drag=Mathf.Lerp(rigid.drag,3.0f,Time.fixedDeltaTime*3);
        else 
            rigid.drag=0;
        Vector2 engineForceVector = transform.up*accelerationInput*accelerationFactor;
        rigid.AddForce(engineForceVector,ForceMode2D.Force);
    }
    private void ApplySteering()
    {
        float minSpeed = rigid.velocity.magnitude/8;
        rotationAngle-=steeringInput*turnFactor*minSpeed;
        rigid.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up*Vector2.Dot(rigid.velocity,transform.up);
        Vector2 rightVelocity = transform.right*Vector2.Dot(rigid.velocity,transform.right);
        rigid.velocity = forwardVelocity + rightVelocity*driftFactor;
    }
    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
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
