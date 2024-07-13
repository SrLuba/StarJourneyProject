using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Global
{
    public static int coin = 0;
    public static int rankingID = 0;


    public static ActorSO FindActorByID(string identifier) {
        if (StaticManager.instance == null) return null;
        ActorSO actorResult = StaticManager.instance.game.actors.Find(x => x.identifier.ToUpper() == identifier.ToUpper());

        if (actorResult == null) return null;

        return actorResult;
    }
    public static ActorSO FindPlayerByPID(int ID) {
        if (StaticManager.instance == null) return null;
        ActorSO actorResult = StaticManager.instance.game.players[ID];

        if (actorResult == null) return null;

        return actorResult;
    }
    public static GameObject FindPlayer(int ID) {
        return null;
    }
}
