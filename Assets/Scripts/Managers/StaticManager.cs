using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;
using static UnityEditor.Recorder.OutputPath;
public enum PlayerTestType { 
    Mario,
    Luigi,
    MarioAndLuigi,
    LuigiAndMario,
    GoombaFRFR
}

public class StaticManager : MonoBehaviour
{
    public static StaticManager instance;

    public bool onBattle;
    public int battleEnteringCase;

    public bool marioAhead = false;

    public int battleAdvantageCase = 0;
    public bool company = false;

    public BattleSO goBattle;

    public GameSO game;

    public int PlayerCount = 2;

    public List<string> bindingsPerPlayerID = new List<string>();

    //public MapSO selectedMap;
    public Discord.Discord discord;
    public Discord.OverlayManager overlayManager;
    public Discord.ActivityManager activityManager;
    public Discord.RelationshipManager relationshipManager;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        // Discord Rich Presence
        discord = new Discord.Discord(1256060167468879903, (UInt64)Discord.CreateFlags.Default);
        this.overlayManager = discord.GetOverlayManager();
        this.activityManager = discord.GetActivityManager();
        this.relationshipManager = discord.GetRelationshipManager();


        // Game
        if (this.game==null) this.game = Resources.Load<GameSO>("Resources/Data/Game");

        this.game.Initialize();

        this.PlayerCount = this.game.currentPlayers.Count;
    
    }
    public void OnApplicationQuit()
    {
        discord.Dispose(); // Discord Rich Presence
    }
    void Start()
    {
        
        DontDestroyOnLoad(this.gameObject); // Make object persistent
     
    }
    public string getInputByPlayerID(int playerID)
    {
        return this.bindingsPerPlayerID[playerID]; 
    }
    private void Update()
    {
        discord.RunCallbacks();

        Activity activity = new Activity();

        activity.Details = "Testing";
        activity.Party = new() { Size = new() { CurrentSize = 1, MaxSize = 1 }, Id = "" };
        activity.State = (string)(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        ActivityAssets assets = new();
        assets.LargeImage = "default";
  
        activity.Assets = assets;


        activityManager.UpdateActivity(activity, (res) => {
            //head empty.
            Debug.Log($"[DISCORD] Rich Presence Update: {res}");
        });

    }
}
