using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UiMenu : UiObject, IOnKeyDown
{
    [Header("UiMenu")]
    public bool isMenuBar;

    private Image image;
    private List<UiMenuLineBase> menuLines;
    private int selectedLine;

    #region MonoBehaviour

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        image = GetComponent<Image>();
        menuLines = new List<UiMenuLineBase>();
    }

    private void Update()
    {
        if (refresh)
            Refresh();
    }

    #endregion

    #region UiObject

    /// <summary>
    /// Reference to the parent
    /// </summary>
    public override void SetFactory(UiMenuFactory factory)
    {
        this.factory = factory;
        image.color = factory.panelColor;
    }

    /// <summary>
    /// Focus on menu will make first line of menu focused
    /// </summary>
    public override void Focus()
    {
        base.Focus();
        SelecLine(FindSeectable(0));
    }


    public override void Close()
    {
        foreach (var line in menuLines)
            line.Close();
        base.Close();
    }

    public override void Refresh()
    {
        foreach (var line in menuLines)
        {
            line.Refresh();        
        }    
        base.Refresh();
    }

    #endregion

    #region LineSelection

    /// <summary>
    /// Find selectable line from start value and to direction
    /// </summary>
    /// <param name="starts"></param>
    /// <param name="reversed"></param>
    /// <returns></returns>
    private int FindSeectable(int starts, bool reversed = false)
    {
        if (menuLines.Count == 0)
            return -1;
        var max = menuLines.Count;

        if (reversed)
        {
            var i = Math.Max(0, Math.Min(starts - 1, max));
            while(i >= 0)
            {
                if (menuLines[i].IsSelectable)
                    return i;
                i--;
            } 
        }
        else
        {
            var i = Math.Max(0, Math.Min(starts + 1, max));
            while (i <= max) 
            {
                if (menuLines[i].IsSelectable)
                    return i;
                i++;
            }
        }
        return starts;
    }

    public void SelectLine(UiMenuLineBase lineBase)
    {
        SelecLine(menuLines.IndexOf(lineBase));
    }

    public void DeselectLine(UiMenuLineBase lineBase)
    {
        lineBase.Defocus();
        selectedLine = -1;
        RequestRefresh();
    }

    private void SelecLine(int index)
    {
        var oldLine = this[selectedLine];
        if (oldLine != null)
            oldLine.Defocus();

        selectedLine = index; 

        var newLine = this[selectedLine];
        if (newLine != null)
            newLine.Focus();
        RequestRefresh();
    }

    #endregion

    #region List of Items

    /// <summary>
    /// Add menu item
    /// </summary>
    /// <param name="lineBase"></param>
    public void Add(UiMenuLineBase lineBase)
    {
        menuLines.Add(lineBase);
        lineBase.parentFocus = this;
        var button = lineBase.GetComponent<UiMenuLine>();
        if (button != null)
            button.menuButton.onClick.AddListener(() => { OnClickMenuItemHandle(lineBase); });
        RequestRefresh();
    }

    /// <summary>
    /// Remove menu item
    /// </summary>
    /// <param name="lineBase"></param>
    public void Remove(UiMenuLineBase lineBase)
    {
        menuLines.Remove(lineBase);
        RequestRefresh();
    }

    public UiMenuLineBase this[int index]
    {
        get
        {
            return (index>= 0 && index < menuLines.Count) ? menuLines[index] : null;
        }
    }

    #endregion

    #region IOnKeyDown

    public override bool OnKeyDown(int evt)
    {

        switch ((KeyCode)VARP.Event.GetKeyCode(evt))
        {
            case KeyCode.DownArrow:
                return OnDownArrow(evt);

            case KeyCode.UpArrow:
                return OnUpArrow(evt);

            case KeyCode.LeftArrow:
                return OnLeftArrow(evt);

            case KeyCode.RightArrow:
                return OnRightArrow(evt);

            case KeyCode.Mouse0:
                return OnRightArrow(evt);
        }
        return false;
    }

    #endregion

    #region Events Handlers

    private bool OnRightArrow(int evt)
    {
        var selected = menuLines[selectedLine];
        if (selected != null)
            selected.OnKeyDown(evt);
        return true;
    }

    private bool OnLeftArrow(int evt)
    {
        Close();
        return true;
    }

    private bool OnUpArrow(int evt)
    {
        SelecLine(FindSeectable(selectedLine, true));
        return true;
    }

    private bool OnDownArrow(int evt)
    {
        SelecLine(FindSeectable(selectedLine, false));
        return true;
    }

    #endregion




    private void OnClickMenuItemHandle(UiMenuLineBase focusLineBase)
    {
        SelectLine(focusLineBase);
        var line = this[selectedLine];
        if (line != null) line.OnKeyDown((int) KeyCode.Mouse0);
    }

    // ReSharper disable once UnusedMember.Local
    private void OnRectTransformDimensionsChange()
    {
        if (isMenuBar)
        {
            var canvasSize = UiManager.I.RectTransform.sizeDelta;
                RectTransform.sizeDelta = new Vector2(canvasSize.x, RectTransform.sizeDelta.y);
        }
        else
        {
            if (parentFocus!=null)
                RectTransform.anchoredPosition = GetFitPosition();
        }
        SelecLine(selectedLine);
    }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    public Vector2 GetFitPosition()
    {
        if (parentFocus == null)
            return Vector2.zero;

        // anchor point
        var canvasSize = UiManager.I.RectTransform.rect.size;
        // submenu size
        var thisRect = RectTransform.rect;
        var thisPos = parentFocus.ChildPosition;
        // There is interesting phenomen. position of object is in top left 
        thisRect.position = new Vector2(thisPos.x, thisPos.y - thisRect.height);

        if (thisRect.Contains(new Vector2(canvasSize.x,thisRect.y)))
        {
            thisRect.position = new Vector2(canvasSize.x - thisRect.width, thisRect.position.y);

            var parentRect = parentFocus.RectTransform.rect;
            var parentPos = parentFocus.PositionOnCanvas;
            // There is interesting phenomen. position of object is in top left 
            parentRect.position = new Vector2(parentPos.x, parentPos.y - parentRect.height);

            if (thisRect.Overlaps(parentRect))
            {


                thisRect.position = new Vector2(thisRect.x, parentRect.yMin - thisRect.height);
                if (thisRect.Contains(new Vector2(thisRect.x, -canvasSize.y)))
                {
                   thisRect.position = new Vector2(thisRect.x, parentRect.yMax);
                }
            }

        }

        thisRect.position = new Vector2(thisRect.position.x, thisRect.position.y + thisRect.height);
        return thisRect.position;
    }
}
