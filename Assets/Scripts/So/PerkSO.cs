
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    PerkSO defines a Perk, like spin, jump, hammer.
    Any action is a perk, including jumping, diving, etc..
 */
[System.Serializable]
public enum PerkType {
    JointAction,
    PhysicsAction
}

[System.Serializable]
public enum PerkPhysicsActionType { 
    
}
[System.Serializable]
public class PerkIncludeCharacter {
    public string identifier = "mario"; // Character identifier, check ActorSO for more information
    public int childID = 0; // childID for taking position
    public Vector3 outOffset; // character offset when respawn
    public AudioClip selectionSpecificClip; // adjacent voice clip or sound that plays when you select the perk per character
}
/*
    Characters are hidden and when you perform a perk, the perk is another controlable character, this will work for multiple characters, most of the time, they become one, and has a ready position, when the AnimationEvent (EndPerk) is called
 */
[CreateAssetMenu]
public class PerkSO : ScriptableObject
{
    public OP_UseRequirement useRequeriment = OP_UseRequirement.Grounded; // USE REQUERIMENT
    public PerkType type;

    public Sprite perkIcon; // HUD Icon that's used when the perk is selected
    public Vector3 offset; // the offset where the perkPrefab appears from the main character.
    public List<PerkIncludeCharacter> perkIncludeCharacters; // Perk Character Events, this is just information containing how Perks affect every character
    public GameObject perkPrefab; // Actual prefab that's spawned
    public AudioClip selectionClip; // generic selection clip, if needed.

   /* public void Perform() {

        List<PlayerOV> oVs = new List<PlayerOV>(GameObject.FindObjectsOfType<PlayerOV>()); // we create a list of the Players and fill it with the objects of type PlayerOV (check PlayerOV.cs for more information)
        List<PlayerOV> oVs2 = new List<PlayerOV>(); // we create another list where we will store the parsed data.

        for (int i = 0; i < oVs.Count; i++)
        {
            if (oVs[i].selfChara.charaType != ActorType.Player) { oVs.RemoveAt(i); } // Perks only works for players for now, so we remove every PlayerOV that isn't a Player
        }

        for (int i = 0; i < perkIncludeCharacters.Count; i++) { // we look through the included characters.
            for (int a = 0; a < oVs.Count; a++)
            {
                if (oVs[i].selfChara.identifier.ToUpper() == perkIncludeCharacters[i].identifier.ToUpper()) { oVs2.Add(oVs[i]); }  // we add to oVs2 list the parsed data.
            }
        }

        if (type == PerkType.PhysicsAction) // we check if the PerkType is a Physics Action, this kind of perktype doesn't change gameobject or remove characters visually, it just overrides physics aspects.
        {
            
            return;
        }
        else if (type == PerkType.JointAction) 
        {
            
            return;
        }
    }*/
}
