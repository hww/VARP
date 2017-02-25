using System;
using System.Collections;
using System.Collections.Generic;
using VARP;
using UnityEngine;
using UnityEngine.UI;

public class UiMenuFactory : MonoBehaviour
{
    [Header("Style")]
    public Color panelColor = Color.black;
    public ColorBlock normalColors;
    [HideInInspector]
    public ColorBlock highlightColors;
    public Color textNormalColor = Color.white;
    public Color textHighlightedColor = Color.black;

    [Header("Prefabs")]
    public GameObject menuPanel;
    public GameObject lineSimpe;
    public GameObject lineToggle;
    public GameObject lineRadio;
    public GameObject separatorNoLine;
    public GameObject separatorSpace;
    public GameObject separatorSingleLine;
    public GameObject separatorDashedLine;

    private void Awake()
    {
        if (Math.Abs(normalColors.colorMultiplier) < 0.1f)
            normalColors.colorMultiplier = 1f;
        highlightColors.normalColor = normalColors.highlightedColor;
        highlightColors.highlightedColor = normalColors.highlightedColor;
        highlightColors.pressedColor = normalColors.pressedColor;
        highlightColors.disabledColor = normalColors.disabledColor;
        highlightColors.colorMultiplier = normalColors.colorMultiplier;
    }

    private UiMenu CreateMenuPanel(Vector3 position, GameObject prefab)
    {
        var panel = Instantiate(prefab);
        panel.transform.SetParent(UiManager.I.transform, false);

        var umenu = panel.GetComponent<UiMenu>();
        umenu.SetFactory(this);
        umenu.Position = position;

        //var rt = umenu.GetComponent<RectTransform>();
        //rt.anchorMin = new Vector2(0, 0.5f);
        //rt.anchorMax = new Vector2(1, 0.5f);
        return umenu;
    }

    public UiMenu CreateMenu(KeyMap menu, Vector3 position, float width = 0, UiObject parent = null)
    {
        var panel = CreateMenuPanel(position, menuPanel);
        panel.parentFocus = parent;

        foreach (var item in menu.items)
        {
            var mitem = item.value;

            if (mitem != null)
            {
                if (mitem is MenuLineBaseSimple)
                {
                    var mitemsimp = (MenuLineBaseSimple)mitem;
                    CreateMenuItem(panel, mitemsimp, lineSimpe);
                }
                else if (mitem is MenuLineBaseComplex)
                {
                    var mitemcomp = (MenuLineBaseComplex)mitem;
                    switch (mitemcomp.buttonType)
                    {
                        case MenuLineBaseComplex.ButtonType.NoButton:
                            CreateMenuItem(panel, mitemcomp, lineSimpe);
                            break;
                        case MenuLineBaseComplex.ButtonType.Toggle:
                            CreateMenuItem(panel, mitemcomp, lineToggle);
                            break;
                        case MenuLineBaseComplex.ButtonType.Radio:
                            CreateMenuItem(panel, mitemcomp, lineRadio);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (mitem is MenuSeparator)
                {
                    var msepar = (MenuSeparator) mitem;
                    switch ((mitem as MenuSeparator).type)
                    {
                        case MenuSeparator.Type.NoLine:
                            CreateSeparator(panel, msepar, separatorNoLine);
                            break;
                        case MenuSeparator.Type.Space:
                            CreateSeparator(panel, msepar, separatorSpace);
                            break;
                        case MenuSeparator.Type.SingleLine:
                            CreateSeparator(panel, msepar, separatorSingleLine);
                            break;
                        case MenuSeparator.Type.DashedLine:
                            CreateSeparator(panel, msepar, separatorDashedLine);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        var rt = panel.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        panel.IsSelected = true;

        return panel;
    }

    public UiMenuLineBase CreateMenuItem(UiMenu menu, MenuLineBase menuLineBase, GameObject prefab)
    {
        var item = Instantiate(prefab);
        item.transform.SetParent(menu.transform, false);

        var uMenuLine = item.GetComponent<UiMenuLine>();
        uMenuLine.SetFactory(this);
        uMenuLine.MenuLineBase = menuLineBase;

        menu.Add(uMenuLine);
        return uMenuLine;
    }

    public UiMenuLineBase CreateSeparator(UiMenu menu, MenuLineBase menuLineBase, GameObject prefab)
    {
        var item = Instantiate(prefab);
        item.transform.SetParent(menu.transform, false);

        var uMenuLine = item.GetComponent<UiMenuLineBase>();
        uMenuLine.SetFactory(this);
        uMenuLine.MenuLineBase = menuLineBase;

        menu.Add(uMenuLine);
        return uMenuLine;
    }
}
