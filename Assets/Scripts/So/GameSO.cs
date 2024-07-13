using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSO : ScriptableObject
{
    public List<ActorSO> actors;

    public List<PickupSO> pickups;
    public List<RankingSO> rankings;
    public List<PerkSO> perks;
    public List<MusicSO> music;


    public List<ActorSO> players;

    public List<string> currentPlayers;

    public BattleEntranceList battleEntranceList;

    public List<PlayerBattlePositionArrangement> playerArrangements = new List<PlayerBattlePositionArrangement>();
    /*
        Battle_GetPosition_Arragement_Player - Get Position from Current Battle Arrangement For Players.
     */
    public List<Vector2> Battle_GetPosition_Arragement_Player(BattleSO battle) {
        int playerCount = StaticManager.instance.PlayerCount;

        PlayerBattlePositionArrangement arrangement = this.playerArrangements.Find(x => x.playerCount == playerCount);

        if (arrangement != null) return arrangement.positions;

        return null;
    }
    /*
        Initialize() - Initialize Game Database
     */
    public void Initialize() { 
        players = new List<ActorSO>();

        foreach (ActorSO actor in actors)
        {
            if (actor.myType == ActorType.Player)
            {
                players.Add(actor);
                actor.currentPlayerID = players.Count - 1;
            }
        }
    }

    public List<string> getPlayersIDS() { 
        return currentPlayers;
    }
    
}
