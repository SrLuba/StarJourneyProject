using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapSection {
    public string identifier;
    public GameObject MapObject;
    public Vector2Int minimapOffset;

    public List<SpawnPointInfo> warpInfo;

    public GameObject Load() {
        GameObject map = MonoBehaviour.Instantiate(MapObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
        return map;
    }
}
public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    
    public string currentSection;
    public MapSection currentMapSection;
    public GameObject currentSectionObject;

    public MapSO map;

    public GameObject mainPlayerMarker, secondaryPlayerMarker;
    public Transform minimapCamera;

    public Transform minimapBG;

    public GameObject minimapParent;
    

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
        if (MusicManager.instance!=null) MusicManager.instance.PlayClip(map.music, true);

        this.map = StaticManager.instance.selectedMap;

        InitializeMap();
    }
    Transform player, player2;
    public void Update()
    {
        if (player == null)
        {
            player = OVManager.instance.mainPlayer.transform;
            return;
        }
        secondaryPlayerMarker.SetActive(OVManager.instance.player2 != null);
        if (OVManager.instance.player2 != null && OVManager.instance.secondaryPlayer != null) {
            if (player2 == null)player2 = OVManager.instance.secondaryPlayer.transform;
            if (player2 != null) secondaryPlayerMarker.transform.position = new Vector3(player2.position.x, 499f, player2.position.z);
        }
        

        mainPlayerMarker.SetActive(true);
        if (player != null) mainPlayerMarker.transform.position = new Vector3(player.position.x, 499f, player.position.z);
        minimapCamera.transform.position = Vector3.Lerp(minimapCamera.transform.position, new Vector3(player.position.x, 500f, player.position.z), 15f*Time.deltaTime);
        minimapBG.position = Vector3.Lerp(minimapBG.position, targetBGPos, 15f * Time.deltaTime);
        minimapParent.SetActive(!StaticManager.instance.onBattle);
    }
    public Vector3 targetBGPos;
    public void InitializeMap() {
        
        this.LoadMap("m0");
        minimapBG = Instantiate(map.Minimap, new Vector3(0f, 0f, 0f), Quaternion.identity).transform;
    }

    public void LoadMap(string goSection)
    {
        currentMapSection = map.sections.Find(x => x.identifier.ToUpper() == goSection.ToUpper());
        currentSectionObject = currentMapSection.Load();
        OVManager.instance.warpInfo = currentMapSection.warpInfo;
        targetBGPos = new Vector3(currentMapSection.minimapOffset.x, 0f, currentMapSection.minimapOffset.y);

        PlayerOV[] ovs = GameObject.FindObjectsByType<PlayerOV>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < ovs.Length; i++)
        {
            if (ovs[i].selfChara.charaType == CharaType.Enemy)
            {
                Destroy(ovs[i].gameObject);
            }
        }
    }

    public void LoadMapSafe(string goSection)
    {
        if (goSection == "") return;
        if (!CheckForChange(goSection)) return;
        if (currentSectionObject == null) return;

        currentMapSection = map.sections.Find(x => x.identifier.ToUpper() == goSection.ToUpper());

        OVManager.instance.warpInfo = currentMapSection.warpInfo;
        Destroy(currentSectionObject);
        currentSectionObject = currentMapSection.Load();
        targetBGPos = new Vector3(currentMapSection.minimapOffset.x, 0f, currentMapSection.minimapOffset.y);

        PlayerOV[] ovs = GameObject.FindObjectsByType<PlayerOV>(FindObjectsInactive.Include, FindObjectsSortMode.None);
       
        for (int i = 0; i < ovs.Length; i++) {
            if (ovs[i].selfChara.charaType == CharaType.Enemy) {
                Destroy(ovs[i].gameObject);
            }
        }


    }
    public bool CheckForChange(string goSection) {
        if (goSection.ToUpper() == currentSection.ToUpper()) return false;

        MapSection goSectionResult = map.sections.Find(x => x.identifier.ToUpper() == goSection.ToUpper());

        return goSectionResult!=null;
    }
}
