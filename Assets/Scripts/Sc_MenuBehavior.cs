using UnityEngine;
using UnityEngine.UI;

public class Sc_MenuBehavior : MonoBehaviour
{
    public GameObject PausaMenuUI;
    public bool JuegoPausado=false;
    public bool acabado=false;

    float currentTime = 0f;
    float startingTime = 1f;
    [SerializeField] Text text;
    [SerializeField] GameObject TimerUI;

    public Sc_Car sc_Car;
    private float velocidad;

    void Start()
    {
        PausaMenuUI.SetActive(false);
        TimerUI.SetActive(true);
        velocidad = sc_Car.Velocidad;
        currentTime=startingTime;
    }
    void Update()
    {
        if(!acabado)
        {
            conteoRegresivo();
        }
        else if (Input.GetKeyDown("escape"))
        {    
            if(JuegoPausado)
                Reanudar();
            else
                Pausa();
        }
    }

    private void Pausa()
    {
        PausaMenuUI.SetActive(true);
        JuegoPausado = true;
        sc_Car.Velocidad=0;

    }
    public void Reanudar()
    {
        PausaMenuUI.SetActive(false);
        JuegoPausado = false;
        sc_Car.Velocidad=velocidad;
    }
    public void Salir()
    {
        ScenesManager.Instance.LoadMain();
    }
    private void conteoRegresivo()
    {
        if(currentTime>0f)
        {
            text.text = currentTime.ToString("0.00");
            currentTime -= Time.deltaTime;
            sc_Car.Velocidad=0;
        }
        else
        {
            currentTime=0;
            TimerUI.SetActive(false);
            sc_Car.Velocidad=velocidad;
            acabado=true;
        }
    }
}
