using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class AirInputManager : MonoBehaviour
{
    public AirInput controller1;
    public AirInput controller2;
    [HideInInspector] public string player1Name, player2Name;
    [HideInInspector] public bool startGame, needMorePlayers, playerLeft, easy, normal, hard;
    private string[] colorNames = new string[] { "#E1E1E1", "#990000"};
    private int colorIndex;
    private int nextDeviceIdQueq;
#if !DISABLE_AIRCONSOLE

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
        AirConsole.instance.onPersistentDataStored += OnPersistentDataStored;
        AirConsole.instance.onPersistentDataLoaded += OnPersistentDataLoaded;

    }

    void Start()
    {
        nextDeviceIdQueq = -1;
        player1Name = "-----";
        player2Name = "-----";
        easy = false;
        normal = false;
        hard = false;
    }

    void OnReady(string code)
    {
        //Initialize Game State
        JObject newGameState = new JObject();
        newGameState.Add("view", new JObject());
        newGameState.Add("playerColors", new JObject());

        AirConsole.instance.SetCustomDeviceState(newGameState);

        
    }

    /// <summary>
    /// We start the game if 2 players are connected and the game is not already running (activePlayers == null).
    /// 
    /// NOTE: We store the controller device_ids of the active players. We do not hardcode player device_ids 1 and 2,
    ///       because the two controllers that are connected can have other device_ids e.g. 3 and 7.
    ///       For more information read: http://developers.airconsole.com/#/guides/device_ids_and_states
    /// 
    /// </summary>
    /// <param name="device_id">The device_id that connected</param>
    void OnConnect(int device_id)
    {
        if (AirConsole.instance.GetControllerDeviceIds().Count > 2)
        {
            SendMessageToController(device_id, "Disabled - Maximal number of players in game.");
            if (AirConsole.instance.GetControllerDeviceIds().Count == 3)
            {
                nextDeviceIdQueq = device_id;
            }
        }

        if (AirConsole.instance.GetActivePlayerDeviceIds.Count == 0 ||
            AirConsole.instance.GetActivePlayerDeviceIds.Count == 1)
        {

            if (AirConsole.instance.GetControllerDeviceIds().Count >= 2)
            {
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[0], "#E1E1E1"));
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[1], "#990000"));
                player1Name = AirConsole.instance.GetNickname(AirConsole.instance.GetControllerDeviceIds()[0]);
                player2Name = AirConsole.instance.GetNickname(AirConsole.instance.GetControllerDeviceIds()[1]);
                StartGame(2);
                needMorePlayers = false;
                playerLeft = false;
            }
            else
            {
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[0], "#E1E1E1"));
                player1Name = AirConsole.instance.GetNickname(AirConsole.instance.GetControllerDeviceIds()[0]);
                StartGame(1);
                needMorePlayers = false;
                playerLeft = false;
            }

            
        }
      
    }

    /// <summary>
    /// If the game is running and one of the active players leaves, we reset the game.
    /// </summary>
    /// <param name="device_id">The device_id that has left.</param>
    void OnDisconnect(int device_id)
    {
        if (AirConsole.instance.GetControllerDeviceIds().Count <= 2)
        {
            if (nextDeviceIdQueq != -1)
            {
                SendMessageToController(nextDeviceIdQueq, "");
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[1], "#990000"));
                nextDeviceIdQueq = -1;
            }
        }

        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        if (active_player != -1)
        {
            if (AirConsole.instance.GetControllerDeviceIds().Count >= 2)
            {
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[0], "#E1E1E1"));
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[1], "#990000"));
                player1Name = AirConsole.instance.GetNickname(AirConsole.instance.GetControllerDeviceIds()[0]);
                player2Name = AirConsole.instance.GetNickname(AirConsole.instance.GetControllerDeviceIds()[1]);
                StartGame(2);
                needMorePlayers = false;
                playerLeft = false;
            }
            else if (AirConsole.instance.GetControllerDeviceIds().Count == 1)
            {
                AirConsole.instance.SetCustomDeviceStateProperty("playerColors", UpdatePlayerColorData(AirConsole.instance.GetCustomDeviceState(0), AirConsole.instance.GetControllerDeviceIds()[0], "#E1E1E1"));
                player1Name = AirConsole.instance.GetNickname(AirConsole.instance.GetControllerDeviceIds()[0]);
                player2Name = "-----";
                StartGame(1);
                needMorePlayers = false;
                playerLeft = false;
            }
            else
            {
                AirConsole.instance.SetActivePlayers(0);
                startGame = false;
                playerLeft = true;
            }
        }
    }

    /// <summary>
    /// We check which one of the active players has moved the paddle.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="data">Data.</param>
    void OnMessage(int device_id, JToken data)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        if (active_player != -1)
        {
            if (active_player == 0)
            {
                controller1.ButtonInput(data);
            }
            if (active_player == 1)
            {
                controller2.ButtonInput(data);
               
            }

        }
    }
   
    public static JToken UpdatePlayerColorData(JToken oldGameState, int deviceId, string colorName)
    {

        //take out the existing playerColorData and store it as a JObject so I can modify it
        JObject playerColorData = oldGameState["playerColors"] as JObject;

        //check if the playerColorData object within the game state already has data for this device
        if (playerColorData.HasValues && playerColorData[deviceId.ToString()] != null)
        {
            //there is already color data for this device, replace it
            playerColorData[deviceId.ToString()] = colorName;
        }
        else
        {
            playerColorData.Add(deviceId.ToString(), colorName);
            //there is no color data for this device yet, create it new
        }

        //logging and returning the updated playerColorData
        return playerColorData;
    }

    public void SendMessageToController(int device_id, string message)
    {
        AirConsole.instance.Message(device_id, message);
    }

    public void SetView(string viewName)
    {
        AirConsole.instance.SetCustomDeviceStateProperty("view", viewName);
    }


    void StartGame(int numberOfPlayer)
    {
        AirConsole.instance.SetActivePlayers(numberOfPlayer);
        startGame = true;
    }
   

    void OnPersistentDataStored(string uid)
    {
        //Log to on-screen Console
        Debug.Log("Stored persistentData for uid " + uid);
    }

    void OnPersistentDataLoaded(JToken data)
    {
       
        string uid = AirConsole.instance.GetUID(AirConsole.instance.GetMasterControllerDeviceId());
        
        if(data[uid] != null)
        {
            if(data[uid]["custom_data"]["win_easy"] != null)
            {
                easy = true;
            }
            else
            {
                easy = false;
            }

            if (data[uid]["custom_data"]["win_normal"] != null)
            {
                normal = true;
            }
            else
            {
                normal = false;
            }

            if (data[uid]["custom_data"]["win_hard"] != null)
            {
                hard = true;
            }
            else
            {
                hard = false;
            }
        }
        else
        {
            Debug.Log("Player not found");
        }
        
        
    }

    public void StorePersistentData(bool w_easy, bool w_normal, bool w_hard)
    {
        //Store test data for the master controller
        JObject testData = new JObject();
        if (w_easy)
        {
            testData.Add("win_easy", true);
        }
        if (w_normal)
        {
            testData.Add("win_normal", true);
        }
        if (w_hard)
        {
            testData.Add("win_hard", true);
        }

        AirConsole.instance.StorePersistentData("custom_data", testData, AirConsole.instance.GetUID(AirConsole.instance.GetMasterControllerDeviceId()));
    }

    public void RequestPersistentData()
    {
        List<string> connectedUids = new List<string>();
        connectedUids.Clear();
        connectedUids.Add(AirConsole.instance.GetUID(AirConsole.instance.GetMasterControllerDeviceId()));
        AirConsole.instance.RequestPersistentData(connectedUids);
    }

    void OnDestroy()
    {

        // unregister airconsole events on scene change
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onReady -= OnReady;
            AirConsole.instance.onMessage -= OnMessage;
            AirConsole.instance.onPersistentDataStored -= OnPersistentDataStored;
            AirConsole.instance.onPersistentDataLoaded -= OnPersistentDataLoaded;
        }
    }
#endif
}
