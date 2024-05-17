using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaManager : MonoBehaviour
{
    public static CharaManager instance;

    public List<CharaSO> characters;
    public List<string> mainCharacters;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public void InitializeEv() {
        for (int i = 0; i < mainCharacters.Count; i++)
        {
            mainCharacters[i] = mainCharacters[i].ToLower();
        }
    }
    void Start()
    {
        InitializeEv();
    }
}
