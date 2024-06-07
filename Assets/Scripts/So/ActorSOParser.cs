using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class Actor {
    public CharaSO mainSO;
    public OVActorSO mainOV;
    public BattleActorSO mainBO;

    string path = "";

    public Actor() { }
    public void Load(string character) { 
        path = Application.dataPath + "/../Data/Actor/"+character.ToLower()+"/actor.char";
        Debug.Log("<color=green>ACTOR PARSER | LOADING " + path + "</color>");
        if (File.Exists(path)) {
            Debug.Log("<color=green>ACTOR PARSER | LOADING " + path + " FOUND</color>");
            ActorParser parser = new ActorParser();
            Actor selfActor = parser.get(File.ReadAllBytes(path));
        }
    }
}
public class ActorParser {

    public Actor selfActor;

    public bool checkHeader(byte[] sourceBytes) {
        byte[] header = new byte[3];
        for (int i = 0; i < 3; i++) header[i] = sourceBytes[i];

        return Encoding.ASCII.GetString(header).ToUpper()=="CHR";
    }
    public Actor get(byte[] bytes) {
        Debug.Log("<color=green>ACTOR PARSER | LOADING CHAR</color>");

        CharaSO mainSO = ScriptableObject.CreateInstance<CharaSO>();
        OVActorSO mainOV = ScriptableObject.CreateInstance<OVActorSO>();
        BattleActorSO mainBO = ScriptableObject.CreateInstance<BattleActorSO>();

        Debug.Log("<color=green>ACTOR PARSER | CHECKING HEADER CHAR</color>");
        if (!checkHeader(bytes)) {
            Debug.Log("<color=red>ERROR LOADING CHARA | HEADER NOT FOUND</color>");
            Debug.Log("<color=red>ACTOR PARSER | ERROR</color>");
            return null;
        }

        Debug.Log("<color=green>ACTOR PARSER | HEADER CHECK DONE!</color>");
        return this.selfActor;
    }
    public ActorParser() { }
}
