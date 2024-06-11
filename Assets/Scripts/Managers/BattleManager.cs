using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public BattleSO assignedBattle;

    public GameObject transitionGB;
    public Animator transitionAnim;

    public Transform PlayerFolder, EnemyFolder, NPCFolder;

    public List<BattleActorSO> bActors;
    public List<BattleActorSO> characterTurnList;

    public List<BattleActorSO> playerActors;
    public List<BattleActorSO> enemyActors;

    public List<GameObject> enemiesG;
    
    public GenericBActor target;

    public BattleActorSO currentTurn;

    public float battleCounter = 0f;

    public Battle_UISelector uiSelector;

    public NumberDisplayer p1HPDisplayer, p2HPDisplayer, p3HPDisplayer;
    public NumberDisplayer p1TPDisplayer, p2TPDisplayer, p3TPDisplayer;

    public List<Battle_UIBlock> blocksUI;

    public bool inputOverride = false;

    public AudioClip liftHammerSFX, releaseHammerSFX;

    public Image bgImage;

    public float bgAlpha;

    public bool cameraBPM = true;

    public bool canBPM = false;

    public MusicSO victoryMusic;
    public GameObject victoryObject;
    public Vector3 victoryObjectOffset;

    public bool victory = false;

    void Awake()
    {
        instance = this;    
    }
    public bool CheckForDefeat()
    {
        int f = bActors.FindIndex(x => x.linkedChara.charaType == CharaType.Player);
        return f < 0;
    }
    public bool CheckForVictory()
    {
        int f = bActors.FindIndex(x => x.linkedChara.charaType == CharaType.Enemy);
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

    public IEnumerator PlayerAction(BattleActorSO cChara, BattleAttackSO attack, string action) {
        canBPM = false;
        Battle_Camera.instance.target = null;
        uiSelector.active = false;

        yield return new WaitForSeconds(.5f);
        BattleManagerNumbers.instance.constantUserValue = 0.5f;
       
        int targetOffset = Random.Range(0, enemyActors.Count - 1);

        target = enemyActors[targetOffset].getInstance().GetComponent<GenericBActor>();
  
        yield return new WaitForSeconds(0.01f);
        yield return attack.Prepare(cChara);

        BattleManagerNumbers.instance.Hurt(BattleUtils.DamageGet(cChara, target.self), target.self);

        yield return InitializeTurnRound();
    }
    public IEnumerator EnemyAction(BattleActorSO cChara)
    {
        canBPM = false;
        uiSelector.active = false;
        BattleUI_Commands.instance.Update_Player_UI(cChara.getInstance().GetComponent<GenericBActor>().tempAttackID);
        Battle_Camera.instance.target = null;

        yield return new WaitForSeconds(2.1f);
        yield return InitializeTurnRound();
    }
    public byte TurnRoundCycle()
    {
        if (characterTurnList.Count <= 0) return 0x00; // 0 = END OF CYCLE

        BattleActorSO turn = characterTurnList[0];

        for (int i = 0; i < blocksUI.Count; i++)
        {
            blocksUI[i].hit = false;
        }

        UpdateVictory();

        if (turn.linkedChara.charaType == CharaType.Player)
        {
            GameObject get = turn.getInstance();
            if (get == null) return 0xFF; // 255 IS ERROR
            uiSelector.target = get.transform;
            Battle_Camera.instance.target = turn;
            uiSelector.active = true;
            canBPM = true;
        }
        else {
            Battle_Camera.instance.target = null;
            uiSelector.active = false;
        }

        for (int i = 0; i < bActors.Count; i++)
        {
            bActors[i].getInstance().GetComponent<GenericBActor>().PrepareForTurn(turn);
            bActors[i].getInstance().GetComponent<GenericBActor>().ShuffleAnimations();
        }

        currentTurn = turn;

        if (turn.linkedChara.charaType == CharaType.Enemy)
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
        int playerID = StaticManager.instance.players.FindIndex(x => x.identifier == identifier);
        if (playerID < 0) return Vector2.zero;

        return (id == 0) ? this.assignedBattle.playersPositions[playerID] : this.assignedBattle.playersPositionsWithTurn[playerID];
    }
    public void StartBattle_SetupMusic() {
     
    }

    public void StartBattle_SetupPlayers() {
        List<BattleActorSO> actors = new List<BattleActorSO>();
        List<string> players = CharaManager.instance.mainPlayers;

        for (int i = 0; i < players.Count; i++) {
            CharaSO actorR = CharaManager.instance.characters.Find(x => x.identifier.ToUpper() == players[i].ToUpper());
            if (actorR!=null) {
                BattleActorSO c = Instantiate(actorR.selfBattle);
                actors.Add(c);
                playerActors.Add(c);
                c.Spawn(c).transform.SetParent(PlayerFolder);
                bActors.Add(c);
                c.name = c.linkedChara.displayName + "| Player_" + i.ToString();
            }
            actorR.selfBattle.dead = false;
        }

    }

    public void StartBattle_SetupEnemies() {
        List<EnemyInformationB> enemies = assignedBattle.enemies;
        enemiesG = new List<GameObject>();
        for (int i = 0; i < enemies.Count; i++) {
            CharaSO re = enemies[i].getChara();
            for (int a = 0; a < enemies[i].charaCount; a++) {
                BattleActorSO res = Instantiate(re.selfBattle);
                Debug.Log("Creating enemy " + res.linkedChara.displayName);
                GameObject g = res.Spawn(res);
                g.transform.SetParent(EnemyFolder);
                enemiesG.Add(g);
                bActors.Add(res);
                enemyActors.Add(res);
                res.dead = false;
                res.name = "Enemy_"+res.linkedChara.displayName + "|" + a.ToString();
                g.name = "Enemy_" + res.linkedChara.displayName + "|" + a.ToString();
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
            cameraBPM = (currentTurn.linkedChara.charaType == CharaType.Player && canBPM);
            if (currentTurn.linkedChara.charaType == CharaType.Player && canBPM) { bgAlpha = 0.5f; }
            else { bgAlpha = 0f; }
        }

        p1TPDisplayer.number = this.playerActors[0].stats.ENERGY.currentValue;
        p2TPDisplayer.number = (this.playerActors.Count >= 2) ? this.playerActors[1].stats.ENERGY.currentValue : 0;
        p3TPDisplayer.number = (this.playerActors.Count >= 3) ? this.playerActors[2].stats.ENERGY.currentValue : 0;

        p1HPDisplayer.number = this.playerActors[0].stats.HEALTH.currentValue;
        p2HPDisplayer.number = (this.playerActors.Count >= 2) ? this.playerActors[1].stats.HEALTH.currentValue : 0;
        p3HPDisplayer.number = (this.playerActors.Count >= 3) ? this.playerActors[2].stats.HEALTH.currentValue : 0;
      
        bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, Mathf.MoveTowards(bgImage.color.a, bgAlpha, 1f*Time.deltaTime));
    }
}
