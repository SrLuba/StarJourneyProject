using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MenuPage {
    public byte id;
    public GameObject parent;
    public List<MenuItemController> items;

}
public class MenuController : MonoBehaviour
{
    public List<MenuPage> pages;
    public Transform cursorPosition;

    public int pageID = 0;
    public int itemID = 0;

    public AudioClip moveSFX, acceptSFX, denySFX;

    public Vector2 offsetCursor;

    public void CursorMove(Vector2 way)
    {
        int addValue = 1;
        if (way == new Vector2(1f, 0f)) {
            addValue = 1;
        }
        else if(way == new Vector2(-1f, 0f)) {
            addValue = -1;
        }
        else if (way == new Vector2(0f, 1f)) {
            addValue = 1;
        }
        else if (way == new Vector2(0f, -1f)) {
            addValue = -1;
        }

        itemID += addValue;

        if (itemID >= pages[this.pageID].items.Count ) itemID = 0;
        if (itemID < 0) itemID = pages[this.pageID].items.Count - 1;

        SoundManager.instance.Play(moveSFX);
    }

    public void ClickAction() {
        MenuItemController co = this.pages[this.pageID].items[this.itemID];
        if (!co.canClick) {
            SoundManager.instance.Play(denySFX);
            return;
        }

        co.Click(this);
        SoundManager.instance.Play(acceptSFX);
    }

    public void ChangePageByID(byte ID) {
        int ind = pages.FindIndex(x => x.id == ID);
        if (ind < 0) return;

        pageID = ind;
        this.itemID = 0;
        LoadMenu();
    }

    public void LoadMenu() {
        for (int i = 0; i < pages.Count; i++) {
            pages[i].parent.SetActive((i == pageID));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.aPress) ClickAction();

        if (InputManager.instance.dpadRightPress || InputManager.instance.dpadDownPress) CursorMove(new Vector2(1f, 0f));
        if (InputManager.instance.dpadLeftPress || InputManager.instance.dpadUpPress) CursorMove(new Vector2(-1f, 0f));
    }

    private void FixedUpdate()
    {
        cursorPosition.position = Vector2.Lerp(cursorPosition.position, (Vector2)this.pages[this.pageID].items[this.itemID].transform.position + offsetCursor, 15f*Time.deltaTime);
    }
}

