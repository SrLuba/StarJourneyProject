using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
}
