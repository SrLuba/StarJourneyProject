using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public BattleSO assignedBattle;


    public GameObject transitionGB;
    public Animator transitionAnim;

    public Transform PlayerFolder, EnemyFolder, NPCFolder;


    public List<GameObject> enemiesG;


   
    
    void Awake()
    {
        instance = this;    
    }

    public void StartBattle_SetupTransition() { 
       transitionGB.SetActive(true);
       transitionAnim.Play("Transition_Off_"+ assignedBattle.enteringCase.ToString() + "_" + (StaticManager.instance.company ? "Company" : "Solo" ) + "_"+StaticManager.instance.battleAdvantageCase.ToString() + "_" + (StaticManager.instance.marioAhead ? "M" : "L"));
    }

    public void StartBattle_SetupMusic() {
   
    }

    public void StartBattle_SetupPlayers() {
        List<BattleActorSO> actors = new List<BattleActorSO>();
        List<string> players = CharaManager.instance.mainCharacters;

        for (int i = 0; i < players.Count; i++) {
            CharaSO actorR = CharaManager.instance.characters.Find(x => x.identifier.ToUpper() == players[i].ToUpper());
            if (actorR!=null) {
                actors.Add(actorR.selfBattle);
                actorR.selfBattle.Spawn(actorR).transform.SetParent(PlayerFolder);
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
    }
    void Start()
    {
        StartBattle();
    }
}
