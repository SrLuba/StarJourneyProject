using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StatTableItem {
    public string ID;
    public LevelTable table;

    public StatTableItem(string ID, LevelTable table) {
        this.ID = ID;
        this.table = table;
    }
}
[System.Serializable]
public class StatTable {
    public List<StatTableItem> characters;

    public StatTable(List<string> lines) {
        List<string> characters = new List<string>();
        this.characters = new List<StatTableItem>();

        lines.RemoveAt(0);

        for (int i = 0; i < lines.Count; i++) {
            bool isNew = characters.FindIndex(x => x == lines[i].Split(',')[0])<0;
            Debug.Log("ASDDDD - " + lines[i].Split(',')[0]);
            Debug.Log(i.ToString() + " ASDDDD 2 - " + lines[i]);

            if (isNew && lines[i].Split(',')[0]!="") { characters.Add(lines[i].Split(',')[0]); }
            if (lines[i] == "") { lines.RemoveAt(i); }
            if (lines[i].Split(',')[0] == "") {lines.RemoveAt(i);}
        }

        for (int i = 0; i < characters.Count; i++) {
       
            List<string> str = new List<string>();
            Debug.Log("<color=red>PARSING - " + characters[i]+"</color>");

            for (int a = 0; a < lines.Count; a++)
            {
                Debug.Log("CHECKING - " + lines[a] + " + " + lines[a].Split(',')[0].ToUpper() + " + " + characters[i].ToUpper());
                if (lines[a].Split(',')[0].ToUpper() == characters[i].ToUpper()) {
                    str.Add(lines[a]);
                    Debug.Log("Deleting" + a.ToString());
                }
            }
            Debug.Log("Left " + str.Count.ToString());
            StatTableItem item = new StatTableItem(characters[i], new LevelTable(new List<string>(str)));

            this.characters.Add(item);
        }
    }
}
public class TableManager : MonoBehaviour
{
    public static TableManager instance;

    public StatTable statTable;
    public List<string> lines;

    public void Awake()
    {
        instance = this;
    }
    public IEnumerator Start() {
        DontDestroyOnLoad(this.gameObject);
        yield return new WaitForSeconds(.1f);
        Debug.Log("TableManager || Getting STATTABLE");
        yield return CSVManager.instance.GetCSV("1fUXsqTmhaDSLcxy2O3W5MMlTSFYy5xUki2kKN7aZ4NQ");

        while (CSVManager.instance.result == null)
        {
            Debug.Log("TableManager || Waiting for STATTABLE | ID==1fUXsqTmhaDSLcxy2O3W5MMlTSFYy5xUki2kKN7aZ4NQ");
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("TableManager || Got STATTABLE");
        this.lines = new List<string>(CSVManager.instance.result);


        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == "") { lines.RemoveAt(i); }
            if (lines[i].Split(',')[0] == "") { lines.RemoveAt(i); }
        }


        statTable = new StatTable(this.lines);
    }
}
