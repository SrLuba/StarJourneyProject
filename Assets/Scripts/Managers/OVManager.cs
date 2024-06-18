using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[System.Serializable]
public class SpawnPointInfo {
    public string identifier;

    [Header("Section that the warp is in")]public string inSection;
    [Header("Section that the out warp is in")] public string outSection;

    public Vector3 spawnPointPos;
    public Vector2 outDirection;
    public Vector2 inDirection;
}


public class OVManager : MonoBehaviour
{
    public static OVManager instance;

    public CharaSO player;
    public CharaSO player2;

    public int playerType = 0;

    public PlayerCameraOV mainCamera;
    public PlayerOV mainPlayer;
    public PlayerOV secondaryPlayer;

    public List<SpawnPointInfo> warpInfo;

    public bool isWarping = false;

    public bool joystick = false;

    public bool p2Enable = false;


    public void Warp(string inSPID, string outSPID) {
        if (this.isWarping) return;

        SpawnPointInfo inf = warpInfo.Find(x => x.identifier.ToUpper() == inSPID.ToUpper());

        this.isWarping = true;
        mainPlayer.SetupInterrupt(false, inf.inDirection);
        
        StartCoroutine(doWarp(inf.outSection, outSPID));
    }

    public IEnumerator doWarp(string goSection, string outWarp) {
    
        if (TransitionManager.instance != null) { TransitionManager.instance.StartTransition("FADE_IN_OUT"); }
        yield return new WaitForSeconds(1f);

        if (MapManager.instance.CheckForChange(goSection))
        {
            MapManager.instance.LoadMapSafe(goSection);
        }

        SpawnPointInfo inf = warpInfo.Find(x => x.identifier.ToUpper() == outWarp.ToUpper());

        Vector2 outDir = inf.outDirection;
        Vector3 outPos = inf.spawnPointPos;

        mainPlayer.transform.position = outPos;
        secondaryPlayer.transform.position = outPos + new Vector3(mainPlayer.animV.x * -1f, 0f, mainPlayer.animV.y * -1f);

        mainCamera.InstantUpdate();
        mainPlayer.SetupInterrupt(outDir);

        yield return new WaitForSeconds(0.75f);
        mainPlayer.SetupInterrupt(true, outDir);
        this.isWarping = false;
       
    }

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public IEnumerator spawnPlayers() {
        yield return new WaitForSeconds(0.002f);


        if (player != null) mainPlayer = player.SpawnOnOverworld(warpInfo.Find(x => x.identifier.ToUpper() == "DEFAULT").spawnPointPos, mainCamera);
        if (player2 != null) secondaryPlayer = player2.SpawnOnOverworld(warpInfo.Find(x => x.identifier.ToUpper() == "DEFAULT").spawnPointPos, mainCamera);
        playerType = (mainPlayer != null && secondaryPlayer != null) ? 2 : 1;


        StaticManager.instance.company = (playerType == 2);
        StaticManager.instance.marioAhead = (mainPlayer.selfChara.identifier.ToUpper() == "MARIO");
    }
    void Start()
    {
        StartCoroutine(spawnPlayers());

    }
    private void Update()
    {
    }
}
