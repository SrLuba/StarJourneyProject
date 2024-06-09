using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public BattleSO assignedBattle;


    public GameObject transitionGB;
    public Animator transitionAnim;

    public Transform PlayerFolder, EnemyFolder, NPCFolder;
    public List<CharaSO> bActors;
    public List<CharaSO> characterTurnList;

    public List<GameObject> enemiesG;

    public CharaSO currentTurn;

    public float battleCounter = 0f;

    public Battle_UISelector uiSelector;
   
    void Awake()
    {
        instance = this;    
    }
    public void InitializeTurnList() {
        List<CharaSO> charactersList = new List<CharaSO>(bActors);

        charactersList.Sort((x, y) => x.stats.SPEED.startValue.CompareTo(y.stats.SPEED.startValue));
        charactersList.Reverse();

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

    
        yield return new WaitForSeconds(0.01f);
    }

    public IEnumerator PlayerAction(string action) {
        yield return new WaitForSeconds(0.01f);
        yield return InitializeTurnRound();
    }   

    public byte TurnRoundCycle()
    {
        if (characterTurnList.Count <= 0) return 0x00; // 0 = END OF CYCLE

        CharaSO turn = characterTurnList[0];

        if (turn.charaType == CharaType.Player)
        {
            GameObject get = turn.selfBattle.getInstance();
            if (get == null) return 0xFF; // 255 IS ERROR
            uiSelector.target = get.transform;
            uiSelector.active = true;
        }
        else {
            uiSelector.active = false;
        }



        for (int i = 0; i < bActors.Count; i++)
        {
            bActors[i].selfBattle.getInstance().GetComponent<GenericBActor>().PrepareForTurn(turn);
        }

        currentTurn = turn;

        if (turn.charaType == CharaType.Enemy)
        {
            BattleUI_Commands.instance.Update_Player_UI(Random.Range(0, 1));
        }
        else
        {
            BattleUI_Commands.instance.Update_Player_UI(-1);
        }

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
        MusicManager.instance.PlayClip(this.assignedBattle.music.musicLoop, true);
    }

    public void StartBattle_SetupPlayers() {
        List<BattleActorSO> actors = new List<BattleActorSO>();
        List<string> players = CharaManager.instance.mainPlayers;

        for (int i = 0; i < players.Count; i++) {
            CharaSO actorR = CharaManager.instance.characters.Find(x => x.identifier.ToUpper() == players[i].ToUpper());
            if (actorR!=null) {
                actors.Add(actorR.selfBattle);
                actorR.selfBattle.Spawn(actorR).transform.SetParent(PlayerFolder);
                bActors.Add(actorR);
            }
        }

    }

    public void StartBattle_SetupEnemies() {
        List<EnemyInformationB> enemies = assignedBattle.enemies;
        enemiesG = new List<GameObject>();
        for (int i = 0; i < enemies.Count; i++) {
            CharaSO re = enemies[i].getChara();
            for (int a = 0; a < enemies[i].charaCount; a++) {
                Debug.Log("Creating enemy " + re.displayName);
                GameObject g = re.selfBattle.Spawn(re);
                g.transform.SetParent(EnemyFolder);
                enemiesG.Add(g);
                bActors.Add(re);
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
    }
}
