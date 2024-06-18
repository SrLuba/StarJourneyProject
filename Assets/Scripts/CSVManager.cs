using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LevelTableItem
{
    public string NameID;
    public int level;
    public int hp, bp, pow, def, speed, luck;
    public int exp;

    public LevelTableItem(string[] data) {
        //        NameID,      Level,         HP,               BP,           POW,            DEF,            SPEED,         LUCK,            EXP
        Debug.Log(data[0]+","+ data[1]+ "," + data[2] + "," + data[3]+ "," + data[4] + "," + data[5]+ "," + data[6] + "," + data[7] + "," + data[8]);
        this.NameID = data[0]; 
        this.level = (data[1] != "") ? int.Parse(data[1]) : 1; 
        this.hp = (data[2] != "") ? int.Parse(data[2]) : 1; 
        this.bp = (data[3] != "") ? int.Parse(data[3]) : 1;
        this.pow = (data[4] != "") ? int.Parse(data[4]) : 1;
        this.def = (data[5] != "") ? int.Parse(data[5]) : 1;
        this.speed = (data[6] != "") ? int.Parse(data[6]) : 1;
        this.luck = (data[7] != "") ? int.Parse(data[7]) : 1;
        this.exp = (data[8] != "") ? int.Parse(data[8]) : 1;
    }
}
[System.Serializable]
public class LevelTable {
    public List<LevelTableItem> items;

    public LevelTable(List<string> lines)
    {
        Debug.Log(lines.Count);
   
        lines.RemoveAt(0);
        
       
        this.items = new List<LevelTableItem>();
        for (int i = 0; i < lines.Count; i++) {
            if (lines[i] == "") continue;

            Debug.Log(lines[i]);
           
            string[] split = lines[i].Split(',');
            LevelTableItem item = new LevelTableItem(split);
            this.items.Add(item);
        }
        //charactersList.Sort((x, y) => x.stats.SPEED.startValue.CompareTo(y.stats.SPEED.startValue));
        this.items.Sort((x, y) => x.level.CompareTo(y.level));
    }
}
public class CSVManager : MonoBehaviour
{
    public static CSVManager instance;

    private const string googleSheetID = "1fUXsqTmhaDSLcxy2O3W5MMlTSFYy5xUki2kKN7aZ4NQ";
    private const string url = "https://docs.google.com/spreadsheets/d/" + googleSheetID + "/?export?format=csv";

    public List<string> result;

    public LevelTable t = null;

    bool ready = false;
    bool reading = false;
    bool gottabledev = false;
    private void Awake()
    {
        instance = this;
    }

    public string getURL(string id) { 
        return  "https://docs.google.com/spreadsheets/d/" + id + "/export?format=csv";
    }

    public IEnumerator GetCSV(string id)
    {
        ready = false;
        reading = true;
        Debug.Log("Getting + " + this.getURL(id));
        UnityWebRequest www = UnityWebRequest.Get(this.getURL(id));
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string l = www.downloadHandler.text;
            string[] result = l.Split(new[] { '\r', '\n' });
            this.result = Utils.StringArrayToList(result);
            Debug.Log("GOT + " + this.result[0]);
            ready = true;
        }
        reading = false;
    }

}
