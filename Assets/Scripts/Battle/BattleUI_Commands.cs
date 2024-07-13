using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI_Commands : MonoBehaviour
{
    public static BattleUI_Commands instance;
    public List<Battle_UI_Command> commands;

    public GameObject commandPrefab;

    int pCount = 0;

    public List<GameObject> statusMeters;

    public List<Vector2> statusMetersPosition;
    public List<Vector2> commandPositions;

    public Transform canvas;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Setup_Player_UI();
    }
    public void Update_Player_UI(int type) {
        if (type == -1) {
            foreach (Battle_UI_Command command in commands) {
                command.active = false;
            }
            return;
        }

        foreach (Battle_UI_Command command in commands)
        {
            command.state = (type == 0) ? "JUMP" : "HAMMER";
            command.active = true;
        }
    }
    public void Setup_Player_UI() {

        pCount = StaticManager.instance.PlayerCount;
        statusMeters.Clear();
        commands.Clear();
        

        for (int i = 0; i < pCount; i++) {

            BattleActorSO B = Global.FindActorByID(StaticManager.instance.game.currentPlayers[i]).selfBattle;
            GameObject statusMeter = Instantiate(B.myBattleUI, canvas);
     
            statusMeter.transform.SetParent(canvas);

            statusMeter.transform.localPosition = Vector3.zero;
            statusMeter.transform.localPosition = statusMetersPosition[i];
            statusMeter.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
          

            GameObject commandObject = Instantiate(this.commandPrefab, canvas);
            
            commandObject.transform.SetParent(canvas);

 
            commandObject.transform.localPosition = Vector3.zero;
            commandObject.transform.localPosition = commandPositions[i];
            commandObject.transform.localScale = new Vector3(5f, 5f, 5f);

            commandObject.GetComponent<Battle_UI_Command>().playerID = StaticManager.instance.game.currentPlayers[i].ToUpper();


            NumberDisplayer hp = statusMeter.transform.GetChild(1).GetComponent<NumberDisplayer>();
            NumberDisplayer tp = statusMeter.transform.GetChild(2).GetComponent<NumberDisplayer>();
            BattleManager.instance.playersBattleActors.Add(new PlayerBattleActor(hp, tp));

            commands.Add(commandObject.GetComponent<Battle_UI_Command>());

            List<Vector2> pos = new List<Vector2>();
            pos.Add(commandPositions[i]);
            pos.Add(commandPositions[i]);

            commandObject.GetComponent<Battle_UI_Command>().position = new List<Vector2>(pos);
            statusMeters.Add(statusMeter);

        }
    }
    
}
