using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class ChampionManage : MonoBehaviour
{
    public GameObject fire1Prefab, fire2Prefab, pressContinue;
    private float time, time2;
    public Transform spawPosition1, spawPosition2, spawPosition3, spawPosition4, spawPosition5;
    private bool fire1active, fire2active, fire3active, fire4active, fire5active;
    public Image championPhoto;
    public Sprite  froggerPhoto,
                    seleninePhoto,
                    drRaptorPhoto,
                    zaraPhoto,
                    elionPhoto,
                    kaaPhoto,
                    haryPhoto,
                    gohmonPhoto;

    private string championName;
    private AirInput airInput1;
    private AirInputManager airInputManager;
    private int highScore;
    public Text highScoreDisplay;
    private string dificult;
    private MusicController musicController;
    private int dificultNivelInt;
    private int timesWhoPlay;
    void Awake()
    {
        airInputManager = GameObject.Find("AirInputManager").GetComponent<AirInputManager>();
        airInput1 = GameObject.Find("Controller1").GetComponent<AirInput>();
        musicController = GameObject.Find("MusicController").GetComponent<MusicController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        musicController.PlayMusic(9);
        airInputManager.SetView("view-1");
        championName = PlayerPrefs.GetString("playerOneCharacter");
        highScore = PlayerPrefs.GetInt("killed") - PlayerPrefs.GetInt("died");
        highScoreDisplay.text = "Report \n Killed: " + PlayerPrefs.GetInt("killed") + "\n Died: " + PlayerPrefs.GetInt("died") + "\n Score: " + highScore;
        dificultNivelInt = PlayerPrefs.GetInt("dificult");
        switch (PlayerPrefs.GetInt("dificult"))
        {
            case 0 :
                dificult = "Easy";
                break;
            case 1:
                dificult = "Normal";
                break;
            case 2:
                dificult = "Hard";
                break;
        }
        timesWhoPlay = 0;

        if(airInputManager.easy)
        {
            timesWhoPlay++;
        }
        if (airInputManager.normal)
        {
            timesWhoPlay++;
        }
        if (airInputManager.hard)
        {
            timesWhoPlay++;
        }


        airInputManager.StorePersistentData(true, false, false);

        pressContinue.SetActive(false);

        if (championName=="Hary")
        {
            championPhoto.sprite = haryPhoto;
        }
        else if (championName == "Zara")
        {
            championPhoto.sprite = zaraPhoto;
        }
        else if (championName == "Elion")
        {
            championPhoto.sprite = elionPhoto;
        }
        else if (championName == "DrRaptor")
        {
            championPhoto.sprite = drRaptorPhoto;
        }
        else if (championName == "Selenine")
        {
            championPhoto.sprite = seleninePhoto;
        }
        else if (championName == "Gohmon")
        {
            championPhoto.sprite = gohmonPhoto;
        }
        else if (championName == "Kaa")
        {
            championPhoto.sprite = kaaPhoto;
        }
        else if (championName == "Frogger")
        {
            championPhoto.sprite = froggerPhoto;
        }

        fire1active = false;
        fire2active = false;
        fire3active = false;
        fire4active = false;
        fire5active = false;
        time = 0;
        time2 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        time2 += Time.deltaTime;

        if(time > 1f && !fire1active)
        {
            Instantiate(fire1Prefab, spawPosition1.position, spawPosition1.rotation);
            fire1active = true;
        }
        if (time > 1.5f && !fire2active)
        {
            Instantiate(fire2Prefab, spawPosition2.position, spawPosition2.rotation);
            fire2active = true;
        }
        if (time > 1.75f && !fire3active)
        {
            Instantiate(fire2Prefab, spawPosition3.position, spawPosition3.rotation);
            fire3active = true;
        }
        if (time > 2f && !fire4active)
        {
            Instantiate(fire1Prefab, spawPosition4.position, spawPosition4.rotation);
            fire4active = true;
        }
        if (time > 2.1f && !fire5active)
        {
            Instantiate(fire1Prefab, spawPosition5.position, spawPosition5.rotation);
            fire5active = true;
        }

        if(fire5active)
        {
            fire1active = false;
            fire2active = false;
            fire3active = false;
            fire4active = false;
            fire5active = false;
            time = 0;
        }

        if (time2 > 3)
        {
            if (!pressContinue.activeSelf)
            {
                pressContinue.SetActive(true);
            }
        }

        if (pressContinue.activeSelf)
        {
            if (airInput1.interact)
            {
                airInput1.interact = false;

                PlayerPrefs.SetInt("playerOneWins", 0);
                PlayerPrefs.SetInt("playerTwoWins", 0);
                PlayerPrefs.SetInt("enemyNumber", 0);
                PlayerPrefs.SetInt("goesToMenu", 0);
                SceneManager.LoadScene("CreditsScene");
            }
        }
    }

}
