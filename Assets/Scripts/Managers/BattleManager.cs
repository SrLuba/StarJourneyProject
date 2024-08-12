using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.UI;
[System.Serializable]public class PlayerBattleActor {
    public NumberDisplayer hp, tp;

    public PlayerBattleActor(NumberDisplayer hp, NumberDisplayer tp)
    {
        this.hp = hp;
        this.tp = tp;
    }
}
public enum BattleState { 
    Idle,
    PlayerTurn,
    EnemyTurn
}
public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public BattleState state;

    public bool TESTING = false;

    public RectTransform targetIcon;
    public Image targetIconImage;

    public BattleSO assignedBattle;

    public GameObject transitionGB;
    public Animator transitionAnim;

    public Transform PlayerFolder, EnemyFolder, NPCFolder;

    public List<BattleActorSO> bActors;
    public List<BattleActorSO> characterTurnList;

    public List<BattleActorSO> playerActors;
    public List<BattleActorSO> enemyActors;

    public BattleActorSO currentPlayerTurn;

    public List<GameObject> enemiesG;
    
    public GenericBActor target;

    public BattleActorSO currentTurn;

    public float battleCounter = 0f;

    public Battle_UISelector uiSelector;

    public List<PlayerBattleActor> playersBattleActors;

    public List<Battle_UIBlock> blocksUI;

    public bool inputOverride = false;

    public AudioClip liftHammerSFX, releaseHammerSFX, uiMoveSFX, uiAcceptSFX;

    public Image bgImage;

    public float bgAlpha;

    public bool cameraBPM = true;

    public bool canBPM = false;

    public MusicSO victoryMusic;
    public GameObject victoryObject;
    public Vector3 victoryObjectOffset;

    public RectTransform mainCanvas;
    public Camera mainCam;

    public bool victory = false;
    public bool testing = true;
    public ActorSpawnLimitationInformation defaultEnemySpawnInformation;

    public string getTeamString() {
        string tString = "";

        foreach (BattleActorSO actor in playerActors)
        {
            tString+=actor.getString().ToUpper();
        }
        Debug.Log("Attempt to get team string : " + tString);
        return tString;
    }
    public string getBattleExt() {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "All Files\0*.*\0\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;//默认路径
        ofn.title = "Open Battle (.ini)";
        ofn.defExt = "INI";//显示文件的类型
                           //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
        if (DllTest.GetOpenFileName(ofn))
        {
            return ofn.file;
        }
        return Application.dataPath + "/../Data/defaultBattle.ini";
    }

    public void LoadBattleAndParse() {


        string[] fileLines = System.IO.File.ReadAllLines(getBattleExt());

        if (fileLines == null) return;


        StaticManager.instance.game.currentPlayers.Clear();

        string[] players = fileLines[1].Split('|');

        for (int i = 0; i < players.Length; i++) {
            ActorSO p = StaticManager.instance.game.players.Find(x => x.identifier.ToUpper() == players[i].ToUpper());
            StaticManager.instance.game.currentPlayers.Add(players[i]);
        }


        BattleSO b = Instantiate(assignedBattle);
        b.enemies.Clear();

        for (int i = 3; i < fileLines.Length; i++) {
            EnemyInformationB info = new EnemyInformationB();

            info.id = fileLines[i].Split('|')[0]; Debug.Log("enemy id : " + info.id);
            info.charaCount = int.Parse(fileLines[i].Split('|')[1]); Debug.Log("enemy count : " + info.charaCount.ToString());
            info.spawnInfo = this.defaultEnemySpawnInformation;
            b.enemies.Add(info);
        }

        this.assignedBattle = b;
    }
    

    void Awake()
    {
        instance = this; 
        LoadBattleAndParse();
    }
    public bool CheckForDefeat()
    {
        int f = bActors.FindIndex(x => x.linkedActor.myType == ActorType.Player);
        return f < 0;
    }
    public bool CheckForVictory()
    {
        int f = bActors.FindIndex(x => x.linkedActor.myType == ActorType.Enemy);
        return f<0 && !CheckForDefeat();
    }
  
    public IEnumerator Win() {
        victory = true;
        CinematicManager.instance.blackLines = false;

        yield return MusicManager.instance.FadePlay(victoryMusic, 6f, 1f);

        for (int i = 0; i < playerActors.Count; i++) {
            if (playerActors[i].dead) continue;
            yield return playerActors[i].getInstance().GetComponent<GenericBActor>().WinCelebration();
        }

        uiSelector.active = false;
        CinematicManager.instance.blackLines = true;
        bgAlpha = .75f;
        this.enabled = false;
    }
    bool canWin = false;
    public void UpdateVictory() {
        if (!CheckForVictory()) return;
        if (!canWin) return;


        StartCoroutine(Win());
        canWin = false;
    }
    public void InitializeTurnList() {
        List<BattleActorSO> charactersList = new List<BattleActorSO>(bActors);

        charactersList.Sort((x, y) => x.stats.SPEED.startValue.CompareTo(y.stats.SPEED.startValue));
        charactersList.Reverse();

        for (int i = 0; i < charactersList.Count; i++)
        {
            if (charactersList[i].dead) charactersList.Remove(charactersList[i]);
        }

        characterTurnList = charactersList;
    }

    public IEnumerator InitializeTurnRound() {
        uiSelector.active = false;
        yield return new WaitForSeconds(0.25f);
      
        byte CYCLE = TurnRoundCycle();

        if (CYCLE == 0xFF) {
            Debug.Log("<color=red>ERROR ON TURN CYCLE</color>");
        }
        else if (CYCLE == 0x01) {
          
        }
        else if (CYCLE == 0x00)
        {
            InitializeTurnList();
            yield return InitializeTurnRound();
        }
        UpdateVictory();

        yield return new WaitForSeconds(0.01f);
    }

    public IEnumerator PlayerAction(BattleActorSO cChara, AttackSO attack, string action) {
        for (int i = 0; i < blocksUI.Count; i++)
        {
            blocksUI[i].hit = true;
        }
        targetIcon.gameObject.SetActive(false);
        canBPM = false;
        Battle_Camera.instance.target = null;
        uiSelector.active = false;
        Battle_Camera.instance.camOverride = false;
        Battle_Camera.instance.target = null;
        Battle_Camera.instance.camOverridePosition = Vector3.zero;
        yield return new WaitForSeconds(1f);
    

        AttackSOItem attac = attack.getAttack(cChara);
        yield return cChara.getInstance().GetComponent<GenericBActor>().IA_Goto_Walk(new Vector2(this.target.transform.position.x, this.target.transform.position.z)+attac.positionOffset, "");
        cChara.getInstance().transform.GetChild(0).gameObject.SetActive(false);

        Vector3 v = target.transform.position + attac.offset;
        GameObject g = Instantiate(attac.attack, new Vector3(v.x, assignedBattle.floorY + attac.offset.y, v.z), Quaternion.identity);
        g.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        g.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

        g.GetComponent<BattleAttackAnimator>().player = cChara;
        g.GetComponent<BattleAttackAnimator>().target = this.target.self;


        target.attendingAttack = true;
        target.linkedAttendingAttackPoint = g.transform.GetChild(1).transform;
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;

        while (g != null) {
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            yield return new WaitForSeconds(0.001f);
        }
        

        cChara.getInstance().transform.GetChild(0).gameObject.SetActive(true);
        yield return cChara.getInstance().GetComponent<GenericBActor>().IA_Goto_Walk(new Vector2(cChara.getInstance().GetComponent<GenericBActor>().normalPosition.x, cChara.getInstance().GetComponent<GenericBActor>().normalPosition.y), "");
        
        target.attendingAttack = false;
        target.linkedAttendingAttackPoint = null;
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.AttendAttackRelease();
        for (int i = 0; i < blocksUI.Count; i++)
        {
            blocksUI[i].hit = false;
        }
        yield return InitializeTurnRound();
    }
    public IEnumerator Targetting(BattleActorSO cChara, AttackSO attack, string action) {

        // Player Enemy Target

        // Setup target image to Actor target image
        targetIcon.gameObject.SetActive(true); // set it visible
        this.targetIconImage.sprite = cChara.targetIconUI;
        this.targetIconImage.SetNativeSize(); // set native size to avoid aspect ratio issues.


        // current selection
        int select = 0;

        this.target = enemyActors[0].getInstance().GetComponent<GenericBActor>();
        
        bool selecting = true; // loop condition.

        CinematicManager.instance.blackLines = true; // we set cinematic black lines (it shows the black lines)
        uiSelector.active = false; // we show the Player UI off


        while (selecting) {
            Battle_Camera.instance.camOverride = true; // we override default camera behaviour
            Battle_Camera.instance.target = this.target.self; // we set the target to the first enemy.
            Battle_Camera.instance.camOverridePosition = turn.getInstance().transform.position; // we set the override position vector 3 to Player's position.
            Battle_Camera.instance.camOverrideOffset = new Vector3(-5f, 2.2f, -3f);

            cChara.getInstance().GetComponent<GenericBActor>().canJump = false; // we disable player from jumping
            cChara.getInstance().GetComponent<GenericBActor>().animationInterrupt = true; // we stop animation behaviour from the player to allow manual animations.
            cChara.getInstance().GetComponent<GenericBActor>().animator.Play("Aim"); // we set the aim animation.

            if (InputManager.instance.engine.getPressed("RIGHT")) // we check if the player has pressed the right button.
            {
                select++;// Needs work, but just adds the current selection by one
                targetIcon.GetComponent<Animator>().Play("Select", 0, 0f); // We play the select animation on the target icon/indicator.
                SoundManager.instance.Play("ui_battle_general|player_selection_move", false); // playing the sound
            }
            if (InputManager.instance.engine.getPressed("LEFT")) // we check if the player has pressed the left button.
            {
                select--; // Needs work, but just substracts the current selection
                targetIcon.GetComponent<Animator>().Play("Select", 0, 0f); // We play the select animation on the target icon/indicator.
                SoundManager.instance.Play("ui_battle_general|player_selection_move", false); // playing the sound
            }

            if (select >= enemyActors.Count)
            {
                select = 0;
            }
            else if (select < 0) { select = enemyActors.Count -1; } // we clamp the value to avoid getting out of the array bounds.

            this.target = enemyActors[select].getInstance().GetComponent<GenericBActor>(); //  we set tge target to the current selection.

            if (currentPlayerTurn.linkedActor.getKey(KeyEventType.Pressed))
            {
        
                cChara.getInstance().GetComponent<GenericBActor>().canJump = true; // we allow the player to jump again
                cChara.getInstance().GetComponent<GenericBActor>().animationInterrupt = false; // disabling animation interrupt
                cChara.getInstance().GetComponent<GenericBActor>().animator.Play("Prepare"); // this can be erased, has no purpose. was meant to be a prepare animation before starting to walk towards the enemy

                SoundManager.instance.Play("ui_battle_general|player_selection_accept", false); // playing the sound
                CinematicManager.instance.blackLines = false; // Disables cinematic black lines
                yield return PlayerAction(cChara, attack, action); // we wait until the action is perform.
                yield break;
            }


            yield return new WaitForSeconds(.001f);
        }
        Battle_Camera.instance.camOverride = false; // this part of the code should be unreachable, but to avoid soft locks, i will leave this here.
        Battle_Camera.instance.target =  null;
        Battle_Camera.instance.camOverridePosition = Vector3.zero;
        CinematicManager.instance.blackLines = false;
    }
    public IEnumerator EnemyAction(BattleActorSO cChara)
    {
        targetIcon.gameObject.SetActive(false);
        canBPM = false;
        uiSelector.active = false;

        List<BattleActorSO> playerListT = new List<BattleActorSO>(playerActors);
        
        // update on AI Targetting, removing dead players!
        for (int i = 0; i < playerListT.Count; i++) {
            if (playerListT[i].dead) playerListT.RemoveAt(i);
        }
        int RNG = Random.Range(0, playerListT.Count);
        Debug.Log("<color=yellow> ENEMY TARGETTING | RNG WAS - " + RNG.ToString() + " - AND PLAYER COUNT IS : " + playerListT.Count.ToString() + "</color>");
        if (RNG > playerListT.Count - 1) RNG = playerListT.Count-1;
        this.target = playerListT[RNG].getInstance().GetComponent<GenericBActor>();
        Debug.Log("<color=yellow> ENEMY TARGETTING | RNG WAS - "+ RNG.ToString() + " - AND PLAYER COUNT IS : "+playerListT.Count.ToString()+"</color>");


       
        Battle_Camera.instance.target = null;
        BattleUI_Commands.instance.Update_Player_UI(cChara.getInstance().GetComponent<GenericBActor>().tempAttackID);

        
        AttackSOItem attac = cChara.attackList[0].getAttack(cChara);
        yield return cChara.getInstance().GetComponent<GenericBActor>().IA_Goto_Walk(new Vector2(this.target.transform.position.x, this.target.transform.position.z) + attac.positionOffset, "");
        cChara.getInstance().transform.GetChild(0).gameObject.SetActive(false);

        Vector3 v = target.transform.position + attac.offset;
        GameObject g = Instantiate(attac.attack, new Vector3(v.x, assignedBattle.floorY+ attac.offset.y,v.z), Quaternion.identity);
        g.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        g.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

        target.attendingAttack = true;
        target.linkedAttendingAttackPoint = g.transform.GetChild(1).transform;

        g.transform.GetChild(0).GetComponent<Battle_Enemy_Attack>().self = cChara;
   

        while (g != null)
        {
       
            yield return new WaitForSeconds(0.001f);
        }
        cChara.getInstance().transform.GetChild(0).gameObject.SetActive(true);
        yield return cChara.getInstance().GetComponent<GenericBActor>().IA_Goto_Walk(new Vector2(cChara.getInstance().GetComponent<GenericBActor>().normalPosition.x, cChara.getInstance().GetComponent<GenericBActor>().normalPosition.y), "");

        target.attendingAttack = false;
        target.linkedAttendingAttackPoint = null;
     
        target.AttendAttackRelease();

        for (int i = 0; i < blocksUI.Count; i++)
        {
            blocksUI[i].hit = false;
        }


        yield return InitializeTurnRound();
    }

    public Vector3 ThinkCameraOffset, ThinkCameraAngle;
    public IEnumerator WaitForPlayerTurn(BattleActorSO turn) {
        GameObject get = turn.getInstance();
        while (!get.GetComponent<GenericBActor>().Grounded) {
            yield return new WaitForSeconds(0.001f);
        }
        uiSelector.target = get.transform;
        uiSelector.active = true;
        canBPM = true;
        currentPlayerTurn = turn;
        Battle_Camera.instance.camOverride = true; // we override default camera behaviour
        Battle_Camera.instance.target = null; // we set the target to the first enemy.
        Battle_Camera.instance.camOverridePosition = turn.getInstance().transform.position; // we set the override position vector 3 to Player's position.
        Battle_Camera.instance.camOverrideOffset = ThinkCameraOffset;
        Battle_Camera.instance.camOverrideAngle = ThinkCameraAngle;
        CinematicManager.instance.blackLines = true;
    }
    BattleActorSO turn;
    public byte TurnRoundCycle()
    {
        if (characterTurnList.Count <= 0) return 0x00; // 0 = END OF CYCLE

        turn = characterTurnList[0];

        if (turn.dead) return 0x00; // 0 = END OF CYCLE IF DEAD



        UpdateVictory();

        if (turn.linkedActor.myType == ActorType.Player)
        {
            StartCoroutine(WaitForPlayerTurn(turn));
        }
        else {
            Battle_Camera.instance.target = null;
            uiSelector.active = false;
            currentPlayerTurn = null;
        }

        for (int i = 0; i < bActors.Count; i++)
        {
            bActors[i].getInstance().GetComponent<GenericBActor>().PrepareForTurn(turn);
            bActors[i].getInstance().GetComponent<GenericBActor>().ShuffleAnimations();
        }

        currentTurn = turn;

        if (turn.linkedActor.myType == ActorType.Enemy)
        {
            StartCoroutine(EnemyAction(turn));
         
        }
        else
        {
            BattleUI_Commands.instance.Update_Player_UI(-1);
        }
        BattleManager.instance.inputOverride = false;
        characterTurnList.RemoveAt(0);

        return 0x01; // 1 = SUCCESS
    }
    public void StartBattle_SetupTransition() { 
       transitionGB.SetActive(true);
       transitionAnim.Play("Transition_Off_"+ assignedBattle.enteringCase.ToString() + "_" + (StaticManager.instance.company ? "Company" : "Solo" ) + "_"+StaticManager.instance.battleAdvantageCase.ToString() + "_" + (StaticManager.instance.marioAhead ? "M" : "L"));
    }
    public Vector2 getPlayerPos(int id, string identifier) {
        int playerID = StaticManager.instance.game.players.FindIndex(x => x.identifier.ToUpper() == identifier.ToUpper());
        if (playerID < 0) return Vector2.zero;

        return StaticManager.instance.game.Battle_GetPosition_Arragement_Player(this.assignedBattle)[playerID];
    }
    public void StartBattle_SetupMusic() {
        if (!TESTING) return;

        BattleEntrance m = StaticManager.instance.game.battleEntranceList.get();
        if (m == null) return;

        MusicManager.instance.PlayClip(m.musicSO, true);
    }
    public void UpdateTargetIcon() {

        if (target == null) return;
        Vector2 ViewportPosition = mainCam.WorldToViewportPoint(target.transform.position);

        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * mainCanvas.sizeDelta.x) - (mainCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * mainCanvas.sizeDelta.y) - (mainCanvas.sizeDelta.y * 0.5f)));

        targetIcon.anchoredPosition = WorldObject_ScreenPosition;
    }
    public void StartBattle_SetupPlayers() {
        List<BattleActorSO> actors = new List<BattleActorSO>();
        List<string> players = new List<string>(StaticManager.instance.game.currentPlayers);

        for (int i = 0; i < players.Count; i++) {
            ActorSO actorR = StaticManager.instance.game.players.Find(x => x.identifier.ToUpper() == players[i].ToUpper());
            if (actorR!=null) {
                BattleActorSO c = Instantiate(actorR.selfBattle);
                actors.Add(c);
                playerActors.Add(c);
                c.Spawn(c).transform.SetParent(PlayerFolder);
                bActors.Add(c);
                c.name = c.linkedActor.displayName + "| Player_" + i.ToString();
            }
            actorR.selfBattle.dead = false;
        }

    }

    public void StartBattle_SetupEnemies() {
        List<EnemyInformationB> enemies = assignedBattle.enemies;
        enemiesG = new List<GameObject>();
        for (int i = 0; i < enemies.Count; i++) {
            ActorSO re = enemies[i].getChara();
            for (int a = 0; a < enemies[i].charaCount; a++) {
                BattleActorSO res = Instantiate(re.selfBattle);
                Debug.Log("Creating enemy " + res.linkedActor.displayName);
                GameObject g = res.Spawn(res);
                g.transform.SetParent(EnemyFolder);
                enemiesG.Add(g);
                bActors.Add(res);
                enemyActors.Add(res);
                res.dead = false;
                res.name = "Enemy_"+res.linkedActor.displayName + "|" + a.ToString();
                g.name = "Enemy_" + res.linkedActor.displayName + "|" + a.ToString();
            }
           
        }
    }

    public void StartBattle() {
        // We Setup Transition.
        StartBattle_SetupTransition();
        // We Setup Music.
        StartBattle_SetupMusic();
        // We Setup Players.
        StartBattle_SetupPlayers();
        // We Setup Enemies.
        StartBattle_SetupEnemies();
        // Order Turn List
        InitializeTurnList();

        // Order Turn List
        StartCoroutine(InitializeTurnRound());
        canWin = true;
    }
    void Start()
    {
        StartBattle();
        
    }
    private void Update()
    {

        //if (Keyboard.current.oKey.wasPressedThisFrame) { StartCoroutine(InitializeTurnRound()); Debug.Log("<color=red>Battle Manager</color> | DEBUG Turn Cycle"); }
        if (battleCounter > 0f)
        {
            battleCounter -= Time.deltaTime;
        }
        else
        {
            battleCounter = 0f;
        }

        if (currentTurn != null) { 
            cameraBPM = (currentTurn.linkedActor.myType == ActorType.Player && canBPM);
            if (currentTurn.linkedActor.myType == ActorType.Player && canBPM) { bgAlpha = 0.5f; }
            else { bgAlpha = 0f; }
        }
        if (playersBattleActors.Count > 0) { 
            for (int i = 0; i < playersBattleActors.Count; i++) {
                this.playersBattleActors[i].hp.number = this.playerActors[i].stats.HEALTH.currentValue;
                this.playersBattleActors[i].tp.number = this.playerActors[i].stats.ENERGY.currentValue;
            }
        }

        bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, Mathf.MoveTowards(bgImage.color.a, bgAlpha, 1f*Time.deltaTime));
        UpdateTargetIcon();
    }
}
