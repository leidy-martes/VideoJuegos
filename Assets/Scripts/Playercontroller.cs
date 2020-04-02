using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playercontroller : MonoBehaviour
{
    

    private Rigidbody rb;
    float movHorizontal, movVertical;
    public float velocidad = 1.0f;
    public float altitud = 100.0f;
    public bool isJump = false;

    public MenuManager menuManager;
    public GameObject puntoInicial, GameOverPanel, felicitacionespanel, uiMobile;
    int diamonds = 0; int lifes = 3;
    public Text lifesText, diamondsText, timeText, lifesfinalText, diamondsfinalText;

    float totalTime = 120f;
    bool pause = false;

    AudioSource audioPlayer;
    public AudioClip pointsSound, jumpSound, DeadSound;

    public bl_Joystick joystick;

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID
          uiMobile.SetActive(true);
        #endif

        rb = GetComponent<Rigidbody>();  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pause)
        {
            CountDown();
        }

        #if UNITY_ANDROID
           movVertical = joystick.Vertical * 0.12f;
           movHorizontal = joystick.Horizontal * 0.12f;
        #endif

        //obtengo los input del teclado
        //movVertical = Input.GetAxis("Vertical");
        //movHorizontal = Input.GetAxis("Horizontal");
        

        //creo mi  vector de movimiento para mi player
        Vector3 movimiento = new Vector3(movHorizontal, 0.0f, movVertical);

        //agregamos fuerza al cuerpo rigido
        rb.AddForce(movimiento * velocidad);

        //saltar la pelota
        if (Input.GetKey(KeyCode.Space) && (!isJump))
        {
            Jump();
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "floor" || collision.gameObject.name == "wood")
        {
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
       if(collider.gameObject.name == "Diamond")
        {
            Destroy(collider.gameObject);
            diamonds += 1;
            diamondsText.text = "0" + diamonds.ToString();

            GetComponent<AudioSource>().clip = pointsSound;
            GetComponent<AudioSource>().Play();

            
        }

       if((collider.gameObject.name == "DeadZone")|| (collider.gameObject.name == "Trampa")) 
        {
            transform.position = puntoInicial.transform.position;
            lifes -= 1;
            lifesText.text = "0" + lifes.ToString();

            if(lifes == 0)
            {
                GameOverGame();
            }

            GetComponent<AudioSource>().clip = DeadSound;
            GetComponent<AudioSource>().Play();
        }

       if(collider.gameObject.name == "final")
        {
            FinishedGame();
        }
    }

    void CountDown()
    {
        totalTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime - (minutes * 60));

        timeText.text = string.Format("{0:0}:{01:00}", minutes, seconds);

        if((minutes == 0) && (seconds == 0))
        {
            GameOverGame();
        }
    }

    public void PauseGame()
    {
        pause = !pause;
        rb.isKinematic = pause;
    }

    void GameOverGame()
    {
        menuManager.GoToMenu(GameOverPanel);
        PauseGame();
    }

    public void RestartGame()
    {
        totalTime = 120f;
        lifes = 3;
        diamonds = 0;
        lifesText.text = "03";
        diamondsText.text = "00";
        rb.isKinematic = false;
        pause = false;
        transform.position = puntoInicial.transform.position;
    }

    void FinishedGame()
    {
        menuManager.GoToMenu(felicitacionespanel);
        lifesfinalText.text = "0" + lifes.ToString();
        diamondsfinalText.text = "0" + diamonds.ToString();
        PauseGame();
    }

    public void Jump()
    {
        if (!isJump)
        {
            Vector3 salto = new Vector3(0, altitud, 0);
            rb.AddForce(salto * velocidad);
            isJump = true;

            GetComponent<AudioSource>().clip = jumpSound;
            GetComponent<AudioSource>().Play();
    
        }

    }
}
