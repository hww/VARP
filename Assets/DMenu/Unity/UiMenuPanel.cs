using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMenuPanel : UiObject
{
    private List<UiMenuLine> menuILines;
    private UiMenuPanel submenu;
    private UiMenuLine submenuLine;

    // ReSharper disable once UnusedMember.Local
    void Awake()
    {
        menuILines = new List<UiMenuLine>();
    }

    public UiMenuLine CreateItem(UiMenu parent, string text, string shortcut, string prefabName = null)
    {
        this.uiMenu = parent;
        var linePrefab = uiMenu.GetPrefab(prefabName ?? "Line");
        var item = GameObject.Instantiate(linePrefab);
        item.transform.SetParent(transform, false);

        var mitem = item.GetComponent<UiMenuSimpleLine>();
        mitem.menuText.text = text;
        mitem.menuShortcut.text = shortcut;
        mitem.menuButton.onClick.AddListener(() => { OnClickMenuItemHandle(mitem); });
        menuILines.Add(mitem);

        var rt = item.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMin = new Vector2(1, 0.5f);
        return mitem;
    }

    public UiMenuLine CreateSeparator(UiMenu parent, string prefabName = null)
    {
        this.uiMenu = parent;
        var linePrefab = uiMenu.GetPrefab(prefabName ?? "MenuSeparator_SingleLine");
        var item = GameObject.Instantiate(linePrefab);
        item.transform.SetParent(transform, false);

        var mitem = item.GetComponent<UiMenuLine>();
        menuILines.Add(mitem);

        var rt = item.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMin = new Vector2(1, 0.5f);
        return mitem;
    }

    private void OnClickMenuItemHandle(UiMenuSimpleLine line)
    {
        Debug.Log(line.menuText.text);
        if (submenu != null)
            uiMenu.DestoryMenu(submenu);

        Vector3 pos = RectTransform.anchoredPosition;
        submenu = uiMenu.CreateTestMenu(pos);
        submenuLine = line;
    }

// ReSharper disable once UnusedMember.Local
void OnDestroy()
    {
        if (submenu != null)
            GameObject.Destroy(submenu.gameObject);
    }

    // ReSharper disable once UnusedMember.Local
    //void LateUpdate()
    //
    //{
    //    if (submenu != null)
    //    {
    //        Vector3 pos = RectTransform.anchoredPosition + submenu.RectTransform.anchoredPosition;
    //        var thiswidth = RectTransform.rect.width;
    //        var submwidth = submenu.RectTransform.rect.width;
    //        var last = pos.x + thiswidth + submwidth;
    //    }
    //}

    private void OnRectTransformDimensionsChange()
    {
        if (uiMenu!=null)
            uiMenu.OnTransformChildChanged(this);
    }

    public void OnTransformSiblingChanged(UiMenuPanel menuPanel)
    {
        if (submenu != menuPanel) return;
        submenu.RectTransform.anchoredPosition = GetFitPosition(submenuLine);
    }

    public Vector2 GetFitPosition(UiMenuLine line)
    {
        // top left corner of the canvas is 0,0
        var canvasSize = uiMenu.RectTransform.sizeDelta;
        // get menu item size
        var itempos = line.PositionOnCanvas;
        var itemsize = line.RectTransform.rect.size;
        var itemmax = itempos + itemsize;
        // submenu size
        var subsize = submenu.Size;
        subsize.y = -subsize.y;         // because canvas origin left top
        var submax = itemmax + subsize;

        var x = submax.x < canvasSize.x ? itemmax.x : itempos.x - subsize.x;
        var y = submax.y > -canvasSize.y ? itempos.y : itempos.y - subsize.y;

        return new Vector2(x, y);
    } 
}
