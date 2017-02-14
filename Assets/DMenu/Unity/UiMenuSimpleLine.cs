using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UiMenuSimpleLine : UiMenuLine, IPointerEnterHandler, IPointerExitHandler
{
    public Button menuButton;
    public Text menuText;
    public Text menuShortcut;
    public Toggle lineToggle;

    /// <summary>
    /// Reference to the parent
    /// </summary>
    public override void SetParent(UiObject parent)
    {
        this.parent = parent;
    }

    /// <summary>
    /// Reference to the parent
    /// </summary>
    public override void SetFactory(UiMenuFactory factory)
    {
        this.factory = factory;
        if (menuButton != null)
            menuButton.colors = factory.colors;
        SetHoverStyle(false);
    }

    void SetHoverStyle(bool hover)
    {
        var manager = UiManager.I;
        if (menuText != null)
            menuText.color = hover ? factory.textHighlightedColor : factory.textNormalColor;

        if (menuShortcut != null)
            menuShortcut.color = hover ? factory.textHighlightedColor : factory.textNormalColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHoverStyle(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHoverStyle(false); 
    }


}

