using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleUtils {
    public static int DamageGet(BattleActorSO a, BattleActorSO b) {
        // Damage Dealt = (Attacker POW)*(Attacker Level)/(Defender DEF) * (Attack Constant). - https://supermariofiles.wordpress.com/2013/02/17/the-inside-story-on-ml-3s-enemy-stats-damage-calculation/#:~:text=Damage%20Dealt%20%3D%20(Attacker%20POW)*(,worth%201.5x%20a%20normal%20attack.)
        float AttackerPOW = a.stats.ATTACK.maxValue;
        int AttackerLevel = a.level;
        float DefenderDef = b.stats.DEFENSE.maxValue;
        float attackConstant = AttackConstantGet();

        return (int)((AttackerPOW * AttackerLevel / DefenderDef) * attackConstant);
    }

    public static float AttackConstantGet() {
        Debug.Log("<color=red>" + ((Global.rankingID + 1) * 0.5f).ToString() + "</color>");
        float result = (Global.rankingID+1) * 1f;
        return result;
    }
}