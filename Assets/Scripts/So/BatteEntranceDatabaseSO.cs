using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleEntranceInfo {
    public List<string> characterCombines;
    public BattleEntranceSO battleEntranceSO;
    public void Parse() {
        for (int i = 0; i < characterCombines.Count; i++) {
            characterCombines[i] = characterCombines[i].ToUpper();
        }
    }
}
[CreateAssetMenu]public class BatteEntranceDatabaseSO : MonoBehaviour
{
    public List<BattleEntranceInfo> entrances;

    public BattleEntranceInfo getCurrentBattle(List<ActorSO> actors) {
        foreach (BattleEntranceInfo entrance in this.entrances)
        {
            entrance.Parse();
        }

        List<string> characters = new List<string>();

        for (int i = 0; i < actors.Count; i++) {
            characters.Add(actors[i].identifier.ToUpper());
        }

        BattleEntranceInfo battleEntranceInfo = this.entrances.Find(x => x.characterCombines == characters);

        return (battleEntranceInfo != null) ? battleEntranceInfo : null;
    }
}
