using System;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VARP;


public class UiMenuLine : UiMenuLineBase, IPointerEnterHandler, IPointerExitHandler
{

    [Header("MenuLineComplex")]
    public Button menuButton;           //< Menu line is a button
    public Text menuText;               //< It has also the text
    public Text menuShortcut;           //< It has shorcut text
    public Toggle lineToggle;           //< It may be addittional toggle button


    private UiMenu submenu;             //< This is opened submenu


    /// <summary>
    /// To redraw this object on the screen
    /// </summary>
    public override void Refresh()
    {

        if (MenuLineBase is MenuLineBaseComplex)
        {
            var line = (MenuLineBaseComplex)MenuLineBase;

            var filtered = line.GetFiltered();
            // texts
            menuText.text = filtered.Text;
            menuShortcut.text = filtered.Shorcut;
            // visibility
            gameObject.SetActive(filtered.IsVisible);
            // enabled disabled
            menuButton.enabled = filtered.IsEnabled;
            // toggle
            if (lineToggle != null)
                lineToggle.isOn = filtered.ButtonState;
        }
        else if (MenuLineBase is MenuLineBaseSimple)
        {
            var line = (MenuLineBaseSimple)MenuLineBase;
            // texts
            menuText.text = line.Text;
            menuShortcut.text = line.Shorcut;
        }

        SetHoverStyle(IsSelected || IsFocused);
    }

    private void SetHoverStyle(bool hover)
    {
        if (menuButton != null)
            menuButton.colors = hover ? factory.highlightColors : factory.normalColors;

        if (menuText != null)
            menuText.color = hover ? factory.textHighlightedColor : factory.textNormalColor;

        if (menuShortcut != null)
            menuShortcut.color = hover ? factory.textHighlightedColor : factory.textNormalColor;
    }

    /// <summary>
    /// Close this element
    /// </summary>
    public override void Close()
    {
        if (submenu != null)
            submenu.Close();
        base.Close();
    }

    /// <summary>
    /// Reference to the parent
    /// </summary>
    public override void SetFactory(UiMenuFactory factory)
    {
        this.factory = factory;
        if (menuButton != null)
            menuButton.colors = factory.normalColors;
        SetHoverStyle(false);
    }



    #region  MouseEvents

    public void OnPointerEnter(PointerEventData eventData)
    {
        //SetHoverStyle(true);
        var pf = parentFocus as UiMenu;
        pf.SelectLine(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //SetHoverStyle(false);
        var pf = parentFocus as UiMenu;
        pf.DeselectLine(this);
    }

    #endregion

    public override void Defocus()
    {
        if (submenu != null)
            submenu.Close();
    }

    public override bool IsSelectable
    {
        get { return true; }
    }


    #region IOnKeyDown

    public override bool OnKeyDown(int evt)
    {
        switch ((KeyCode) VARP.Event.GetKeyCode(evt))
        {
            case KeyCode.DownArrow:
                break;

            case KeyCode.UpArrow:
                break;

            case KeyCode.LeftArrow:
                break;

            case KeyCode.RightArrow:
                return OnRightArrow();

            case KeyCode.Return:
                return OnRightArrow();

            case KeyCode.Mouse0:
                return OnRightArrow();
        }
        return false;
    }

    private bool OnRightArrow()
    {
        object binding = null;

        if (MenuLineBase is MenuLineBaseSimple)
        {
            var m = (MenuLineBaseSimple)MenuLineBase;
            binding = m.binding;
        }
        else if (MenuLineBase is MenuLineBaseComplex)
        {
            var m = (MenuLineBaseComplex)MenuLineBase;
            binding = m.binding;
        }
        else
        {
            parentFocus.Close();
            return true;
        }

        if (binding == null)
        {
            parentFocus.Close();
            return true;
        }
        else if (binding is KeyMap)
        {
            var menu = (KeyMap)binding;
            Vector3 pos = ChildPosition;
            submenu = UiManager.I.CreateMenu(menu, pos, 200, this);

        }
        else if (binding is NativeFunction)
        {
            var func = (NativeFunction)binding;
            func.Call();
            parentFocus.Close();
        }
        else if (binding is string)
        {
            var expression = (string)binding;
            var func = NativeFunction.Lockup(expression);
            func.Call();
            parentFocus.Close();
        }
        return true;
    }

    #endregion


}




