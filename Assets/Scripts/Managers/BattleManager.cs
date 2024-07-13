using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
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

    

    void Awake()
    {
        instance = this;    
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
        uiSelector.active = false;
        MusicManager.instance.StopAll();
        yield return new WaitForSeconds(1f);
        MusicManager.instance.PlayClip(victoryMusic, true);
        Instantiate(victoryObject, new Vector3(victoryObjectOffset.x, victoryObjectOffset.y, victoryObjectOffset.z), Quaternion.identity).transform.eulerAngles = new Vector3(0f, 90f, 0f);
        Battle_Camera.instance.gameObject.SetActive(false);

        this.gameObject.AddComponent<AudioListener>();

        for (int i = 0; i < playerActors.Count; i++) {
            Destroy(playerActors[i].getInstance());
        }
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

        for (int i = 0; i < charactersList.Count; i++) {
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
        targetIcon.gameObject.SetActive(true);
        this.target = enemyActors[0].getInstance().GetComponent<GenericBActor>();
        int select = 0;
        bool selecting = true;
        CinematicManager.instance.blackLines = true;
        uiSelector.active = false;
        while (selecting) {
            Battle_Camera.instance.camOverride = true;
            Battle_Camera.instance.target = this.target.self;
            Battle_Camera.instance.camOverridePosition = turn.getInstance().transform.position;

            cChara.getInstance().GetComponent<GenericBActor>().canJump = false;
            cChara.getInstance().GetComponent<GenericBActor>().animationInterrupt = true;
            cChara.getInstance().GetComponent<GenericBActor>().animator.Play("Aim");

            if (InputManager.instance.engine.getPressed("RIGHT")) 
            {
                select++;
                targetIcon.GetComponent<Animator>().Play("Select", 0, 0f);
                SoundManager.instance.Play(uiMoveSFX);
            }
            if (InputManager.instance.engine.getPressed("LEFT"))
            {
                select--;
                targetIcon.GetComponent<Animator>().Play("Select", 0, 0f);
                SoundManager.instance.Play(uiMoveSFX);
            }

            if (select >= enemyActors.Count)
            {
                select = 0;
            }
            else if (select < 0) { select = enemyActors.Count -1; }

            this.target = enemyActors[select].getInstance().GetComponent<GenericBActor>();

            if (currentPlayerTurn.linkedActor.getKey(KeyEventType.Pressed))
            {
        
                cChara.getInstance().GetComponent<GenericBActor>().canJump = true;
                cChara.getInstance().GetComponent<GenericBActor>().animationInterrupt = false;
                cChara.getInstance().GetComponent<GenericBActor>().animator.Play("Prepare");

                SoundManager.instance.Play(uiAcceptSFX);
                CinematicManager.instance.blackLines = false;
                yield return PlayerAction(cChara, attack, action);
                yield break;
            }


            yield return new WaitForSeconds(.001f);
        }
        Battle_Camera.instance.camOverride = false;
        Battle_Camera.instance.target =  null;
        Battle_Camera.instance.camOverridePosition = Vector3.zero;
        CinematicManager.instance.blackLines = false;
    }
    public IEnumerator EnemyAction(BattleActorSO cChara)
    {
        targetIcon.gameObject.SetActive(false);
        canBPM = false;
        uiSelector.active = false;
        this.target = playerActors[Random.Range(0, playerActors.Count - 1)].getInstance().GetComponent<GenericBActor>();

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

        yield return new WaitForSeconds(2.1f);
        yield return InitializeTurnRound();
    }
    public IEnumerator WaitForPlayerTurn(BattleActorSO turn) {
        GameObject get = turn.getInstance();
        while (!get.GetComponent<GenericBActor>().Grounded) {
            yield return new WaitForSeconds(0.001f);
        }
        uiSelector.target = get.transform;
        Battle_Camera.instance.target = turn;
        uiSelector.active = true;
        canBPM = true;
        currentPlayerTurn = turn;
    }
    BattleActorSO turn;
    public byte TurnRoundCycle()
    {
        if (characterTurnList.Count <= 0) return 0x00; // 0 = END OF CYCLE

        turn = characterTurnList[0];


     
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
        int playerID = StaticManager.instance.game.players.FindIndex(x => x.identifier == identifier);
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
        if (Keyboard.current.oKey.wasPressedThisFrame) { StartCoroutine(InitializeTurnRound()); Debug.Log("<color=red>Battle Manager</color> | DEBUG Turn Cycle"); }
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
