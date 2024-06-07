using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PickupType { 
    Item,
    Coin
}

[CreateAssetMenu]
public class PickupSO : ScriptableObject
{
    public PickupType pickup;
    public Sprite icon;

    public GameObject model;
    public GameObject selfParticle;
    public AudioClip hitClip;
    public AudioClip secondaryHitClip;
    public int moneyValue;
    public int sellValue;
    public int buyValue;
}
