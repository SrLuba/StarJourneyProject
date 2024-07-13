using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    Hey, this is the code that contains the mayority of actors variables and everything, please read this as a whole.

    Actors are the main type of instance/object of the game that has some behaviour, ActorSO it's the main one that represents every character.
    It uses some libraries to work, specially on inputs, LEInput is a library that extends the input functionality and makes it easier to implement
 */


[CreateAssetMenu]
public class ActorSO : ScriptableObject
{
    // Every actor has an identifier that is converted to uppercase every time is checked to avoid errors on case mistakes.
    // identifier is unique
    public string identifier;
    public string displayName = "?";

    public float size;
    public Vector3 selfShadowOffset;

    // Every actor has a type that makes it work correctly.
    // There's some useful types, and scripts are arranged depending on which type you choose
    // ActorType.Player sets up a Player, quite self explanatory, and changes scripts used for more optimization.
    public ActorType myType;

    public int playerID;

    public int currentPlayerID = 0;



    // Perks! please check PerkSO for more information about this files.
    public List<PerkSO> perks;


    public float floorYOffset;

    public ColliderInformation collider;
    public LayerMask floorMask;
    [Header("Character Overworld")] public OVActorSO OVActor;
    public BattleActorSO selfBattle;


    // bool getKey - returns bool of the key pressed.
    public bool getKey(KeyEventType type) {
        switch (type)
        {
            case KeyEventType.Pressed:
                return InputManager.instance.engine.getInputBinding(StaticManager.instance.getInputByPlayerID(this.playerID)).getPress();
            case KeyEventType.Released:
                return InputManager.instance.engine.getInputBinding(StaticManager.instance.getInputByPlayerID(this.playerID)).getReleased();
            case KeyEventType.Holding:
                return InputManager.instance.engine.getInputBinding(StaticManager.instance.getInputByPlayerID(this.playerID)).getHold();
        }
        return false;
    }
    // Transform FindInstance - Seeks for the Game Object instance of the Actor 
    public Transform FindInstance()
    {
        GameObject g = GameObject.Find("OVACTOR_" + identifier.ToUpper());
        return (g != null) ? g.transform : null;
    }

    public ActorOV SpawnOnOverworld(Vector3 spawnPoint) { return null; }
}
