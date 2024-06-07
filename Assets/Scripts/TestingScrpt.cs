using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScrpt : MonoBehaviour
{
    public string charaName;

    public void Start()
    {
        Actor act = new Actor();
        act.Load(charaName);
    }
}
