using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public CharaSO enemy;

    public void Start()
    {
        if (enemy.charaType != CharaType.Enemy) return;
        enemy.SpawnOnOverworld(this.transform.position, OVManager.instance.mainCamera);
    }
}
