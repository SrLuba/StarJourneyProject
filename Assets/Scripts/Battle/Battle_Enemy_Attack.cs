using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Enemy_Attack : MonoBehaviour
{
    public void End() {
        Destroy(this.gameObject);
    }
}
