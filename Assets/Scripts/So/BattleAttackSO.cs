using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BattleAttackGoal { 
    Land,
    PlayerImpact,
    HammerImpact,
    Impact
}
[System.Serializable]
public class BattleAttackSequenceItem {
    public float delay = 0f;

    public float inputEventDelay = 0f;
    public float inputEventDuration = 0.2f;

    public byte successID = 0x01;
    public byte failID = 0xFF;

    public bool timeEvent = false;

    public Vector2 PositionGo = Vector2.zero;
    public bool targetLock = true;
    public string flag = "JUMP";

    public bool asyncMove = true;

    public float pointValue = 0.1f;

    public IEnumerator Execute(BattleActorSO actor, BattleAttackSequence sequence) { 
      

        GenericBActor ractor = actor.getInstance().GetComponent<GenericBActor>();

        if (flag.ToUpper() == "JUMP") { ractor.Jump(); }

        Vector2 position = this.PositionGo;
        yield return new WaitForSeconds(delay);

        if (targetLock) {
            Vector3 a = BattleManager.instance.target.transform.position;
            position = this.PositionGo + new Vector2(a.x, a.z); 
        }

        if (asyncMove)
        {
            ractor.StartCoroutine(ractor.IA_Goto_Walk(position, "Idle"));
        }
        else {
            yield return ractor.IA_Goto_Walk(position, "Idle");
        }


        if (timeEvent)
        {
            yield return new WaitForSeconds(inputEventDelay);
            if (actor.linkedChara.getActionDown())
            {
                sequence.parent.currentSequenceID = successID;
                Success();
                ractor.StartCoroutine(ractor.IA_Goto_Walk(ractor.normalPosition, "Idle"));
                yield return null;
                yield break;
            }
            yield return new WaitForSeconds(inputEventDuration);
            sequence.parent.currentSequenceID = failID;
            Fail();
        }
        else {
            Success();
        }
       
       

    }
    public void Success() {
        Debug.Log("<color=red>SUCCESS ATTACK</color>");
        BattleManagerNumbers.instance.constantUserValue += pointValue;
    }
    public void Fail() {
        Debug.Log("<color=red>FAILED ATTACK</color>");
    }
}
[System.Serializable]
public class BattleAttackSequence {
    public byte sequenceID;
    [HideInInspector] public BattleAttackSO parent;

    public bool inputOverride = false;

    public List<BattleAttackSequenceItem> items;

    public void Initialize(BattleAttackSO parent) {
        this.parent = parent;
    }

    public IEnumerator ExecuteSequence(BattleActorSO actor) {
        BattleManager.instance.inputOverride = this.inputOverride;

        for (int i = 0; i < items.Count; i++) {
            yield return items[i].Execute(actor, this);
        }

        yield return new WaitForSeconds(1f); 
    }
}
[CreateAssetMenu]
public class BattleAttackSO : ScriptableObject
{
    public BattleAttackGoal goal;
   [HideInInspector] public byte currentSequenceID = 0x00;

    public List<BattleAttackSequence> sequences;
    public IEnumerator Prepare(BattleActorSO actor) {
        currentSequenceID = 0x00;
        for (int i = 0; i < sequences.Count; i++) sequences[i].Initialize(this);
        yield return sequences[currentSequenceID].ExecuteSequence(actor);
    }

    
}
