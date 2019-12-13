nusing System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuController : MonoBehaviour {
	private int select;
	public Button btnOnePlayer, btnTwoPlayer, btnAbout;
	public Sprite selectedButton, normalButton;
	public AudioClip clickButtonSound, menuSelectSound;
    private bool soundSelectPlay;
    AudioSource audioSource;
	private bool click, click2;
    private AirInput airInput1, airInput2;
    private AirInputManager airInputManager;
    private MusicController musicController;

    void Awake(){
        airInputManager = GameObject.Find("AirInputManager").GetComponent<AirInputManager>();
        airInput1 = GameObject.Find("Controller1").GetComponent<AirInput>();
        airInput2 = GameObject.Find("Controller2").GetComponent<AirInput>();
        musicController = GameObject.Find("MusicController").GetComponent<MusicController>();
        Time.timeScale = 1f;
        if (!PlayerPrefs.HasKey("numberOfPlayers")) PlayerPrefs.SetInt("numberOfPlayers", 1);
        if (!PlayerPrefs.HasKey("dificult")) PlayerPrefs.SetInt("dificult", 1);
        if (!PlayerPrefs.HasKey("playerOneWins")) PlayerPrefs.SetInt("playerOneWins",0);
		if(!PlayerPrefs.HasKey("playerTwoWins")) PlayerPrefs.SetInt("playerTwoWins",0);
		if(!PlayerPrefs.HasKey("timeLeft")) PlayerPrefs.SetInt("timeLeft",179);
		if(!PlayerPrefs.HasKey("playerOneCharacter")) PlayerPrefs.SetString("playerOneCharacter","Zara");
		if(!PlayerPrefs.HasKey("playerTwoCharacter")) PlayerPrefs.SetString("playerTwoCharacter","Zara");
        if(!PlayerPrefs.HasKey("enemyNumber")) PlayerPrefs.SetInt("enemyNumber", 0);
        if(!PlayerPrefs.HasKey("killed")) PlayerPrefs.SetInt("killed", 0);
        if(!PlayerPrefs.HasKey("died")) PlayerPrefs.SetInt("died", 0);
        if(!PlayerPrefs.HasKey("goesToMenu")) PlayerPrefs.SetInt("goesToMenu", 0);
    }

    
	// Use this for initialization
	void Start () {
        musicController.PlayMusic(0);
        musicController.gameObject.GetComponent<AudioSource>().mute = false;
        airInputManager.SetView("view-1");
		PlayerPrefs.SetInt("playerOneWins",0);
		PlayerPrefs.SetInt("playerTwoWins",0);
        PlayerPrefs.SetInt("enemyNumber", 0);
        PlayerPrefs.SetInt("timeLeft",179);
        PlayerPrefs.SetInt("killed", 0);
        PlayerPrefs.SetInt("died", 0);
        click = false;
		select = 0;
		audioSource = GetComponent<AudioSource>();

        //Request User data
        airInputManager.RequestPersistentData();
    }

	void Update()
	{

        KeyboardControlMenu();
    }

	

	public void KeyboardControlMenu()
	{
        if ((airInput1.movingUp) && !click)
        {
            click = true;
            select--;
            if (select < 0)
            {
                select = 2;
            }
            audioSource.PlayOneShot(clickButtonSound);
        }
        else if (!airInput1.movingUp)
        {
            click = false;
        }

        if ((airInput1.movingDown) && !click2)
        {
            click2 = true;
            select++;
            if (select > 2)
            {
                select = 0;
            }
            audioSource.PlayOneShot(clickButtonSound);
        }
        else if (!airInput1.movingDown)
        {
            click2 = false;
        }


       
        if (select==0)
		{
            btnOnePlayer.GetComponent<Image>().sprite = selectedButton;
		}else
		{
            btnOnePlayer.GetComponent<Image>().sprite = normalButton;
		}


		if(select==1)
		{
            btnTwoPlayer.GetComponent<Image>().sprite = selectedButton;
		}else
		{
            btnTwoPlayer.GetComponent<Image>().sprite = normalButton;
		}

        if (select == 2)
        {
            btnAbout.GetComponent<Image>().sprite = selectedButton;
        }
        else
        {
            btnAbout.GetComponent<Image>().sprite = normalButton;
        }

        if (airInput1.interact)
        {
            airInput1.interact = false;
            if (!soundSelectPlay)
            {
                audioSource.PlayOneShot(menuSelectSound);
                soundSelectPlay = true;
            }

            switch (select)
            {
                case 0:
                    PlayerPrefs.SetInt("numberOfPlayers", 1);
                    StartCoroutine(LoadLevelAfterDelay("SelectPlayerScreen", 1));
                    break;
                case 1: 
                    PlayerPrefs.SetInt("numberOfPlayers", 2);
                    StartCoroutine(LoadLevelAfterDelay("SelectPlayerScreen", 1));
                    break;
                case 2:
                    PlayerPrefs.SetInt("goesToMenu", 1);
                    StartCoroutine(LoadLevelAfterDelay("CreditsScene", 1));
                    break;
            }
            
        }
	}
	
	public void StartGame()
	{
		SceneManager.LoadScene("SelectPlayerScreen");
	}


	public void ExitGame()
	{
		Application.Quit();
	}

    IEnumerator LoadLevelAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
