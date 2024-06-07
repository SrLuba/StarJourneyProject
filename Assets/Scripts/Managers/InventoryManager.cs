using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class InventoryItem
{
    public PickupSO pickup;
    public int count = 0;

    public InventoryItem(PickupSO type) {
        pickup = type;
        this.count = 0;
    }
}

[System.Serializable]
public class InventoryBag {
    public List<InventoryItem> items;
}


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public InventoryBag bag;


    public Animator bagAnimator;
    public Image bagItemSprite;


    public void AddItem(PickupSO type, int count) {
        int item = bag.items.FindIndex(x => x.pickup == type);

        if (item < 0) {
            bag.items.Add(new InventoryItem(type));
            item = bag.items.Count-1;
        }

        bag.items[item].count += count;

        if (type.pickup == PickupType.Item) {
            if (bagAnimator.GetCurrentAnimatorStateInfo(0).IsName("AddBag")) return;
            bagItemSprite.sprite = type.icon;
            bagAnimator.Play("AddBag");
        }
    }

    public void UseItem(PickupSO type, int count)
    {
        int item = bag.items.FindIndex(x => x.pickup == type);

        if (item < 0)
        {
            return;
        }

        bag.items[item].count -= count;
    }

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
