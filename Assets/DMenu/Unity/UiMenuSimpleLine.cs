using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UiMenuSimpleLine : UiMenuLine, IPointerEnterHandler, IPointerExitHandler
{
    public Button menuButton;
    public Text menuText;
    public Text menuShortcut;
    public Toggle lineToggle;

    protected override void OnSetUiMenu()
    {
        if (UiManager == null) return;
        if (menuButton != null)
            menuButton.colors = UiManager.colors;
        SetHoverStyle(false);
    }

    void SetHoverStyle(bool hover)
    {
        if (UiManager == null) return;

        if (menuText != null)
            menuText.color = hover ? UiManager.textHighlightedColor : UiManager.textNormalColor;

        if (menuShortcut != null)
            menuShortcut.color = hover ? UiManager.textHighlightedColor : UiManager.textNormalColor;
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

