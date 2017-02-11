using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMenu : UiObject
{
    private Image image;
    private List<UiMenuLine> menuILines;
    private UiMenu submenu;
    private UiMenuLine submenuLine;

    // ReSharper disable once UnusedMember.Local
    void Awake()
    {
        image = GetComponent<Image>();
        menuILines = new List<UiMenuLine>();
    }

    protected override void OnSetUiMenu()
    {
        if (UiManager == null) return;
        image.color = UiManager.panelColor;
    }

    public void Add(UiMenuLine line)
    {
        menuILines.Add(line);
    }

    public void Remove(UiMenuLine line)
    {
        menuILines.Remove(line);
    }

    private void OnClickMenuItemHandle(UiMenuSimpleLine line)
    {
        Debug.Log(line.menuText.text);
        if (submenu != null)
            UiManager.DestoryMenu(submenu);

        Vector3 pos = RectTransform.anchoredPosition;
       // submenu = UiMenu.CreateTestMenu(pos);
        //submenuLine = line;
    }

    // ReSharper disable once UnusedMember.Local
    void OnDestroy()
    {
        if (submenu != null)
            GameObject.Destroy(submenu.gameObject);
    }

    // ReSharper disable once UnusedMember.Local
    private void OnRectTransformDimensionsChange()
    {
        if (UiManager!=null)
            UiManager.OnTransformChildChanged(this);
    }

    public void OnTransformSiblingChanged(UiMenu menu)
    {
        if (submenu != menu) return;
        submenu.RectTransform.anchoredPosition = GetFitPosition(submenuLine);
    }

    public Vector2 GetFitPosition(UiMenuLine line)
    {
        // top left corner of the canvas is 0,0
        var canvasSize = UiManager.RectTransform.sizeDelta;

        // get menu item size
        //
        // pos +--------------------------+
        //     |                          |
        //     +--------------------------+ max
        var linepos = line.PositionOnCanvas;
        var linesize = line.RectTransform.rect.size;
        var linemax = linepos + linesize;

        // submenu size
        var submenusize = submenu.RectSize;
        submenusize.y = -submenusize.y;         // because canvas origin left top
        var submenumax = linemax + submenusize;


        var x = submenumax.x < canvasSize.x ? linemax.x : linepos.x - submenusize.x;
        var y = submenumax.y > -canvasSize.y ? linepos.y : linepos.y - submenusize.y - linesize.y;

        return new Vector2(x, y);
    } 
}
