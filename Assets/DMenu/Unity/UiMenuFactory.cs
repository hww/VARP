using System;
using System.Collections;
using System.Collections.Generic;
using DMenu;
using UnityEngine;
using UnityEngine.UI;

public class UiMenuFactory : MonoBehaviour
{
    [Header("Style")]
    public Color panelColor = Color.black;
    public ColorBlock colors;
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


    public UiMenu CreateMenu(Menu menu, Vector3 position, float width = 0)
    {
        var panel = CreateMenuPanel(position, menuPanel);

        foreach (var item in menu.items)
        {
            var mitem = item.value as MenuItem;

            if (mitem != null)
            {
                if (mitem is MenuItemSimple)
                {
                    if (mitem is MenuItemComplex)
                    {
                        var mitemcomp = mitem as MenuItemComplex;
                        switch (mitemcomp.buttonType)
                        {
                            case MenuItemComplex.ButtonType.NoButton:
                                CreateMenuItem(panel, mitem.Text, mitem.Shorcut, lineSimpe);
                                break;
                            case MenuItemComplex.ButtonType.Toggle:
                                CreateMenuItem(panel, mitem.Text, mitem.Shorcut, lineToggle);
                                break;
                            case MenuItemComplex.ButtonType.Radio:
                                CreateMenuItem(panel, mitem.Text, mitem.Shorcut, lineRadio);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                        CreateMenuItem(panel, mitem.Text, mitem.Shorcut, lineSimpe);
                }

                else if (mitem is MenuSeparator)
                {
                    switch ((mitem as MenuSeparator).type)
                    {
                        case MenuSeparator.Type.NoLine:
                            CreateSeparator(panel, separatorNoLine);
                            break;
                        case MenuSeparator.Type.Space:
                            CreateSeparator(panel, separatorSpace);
                            break;
                        case MenuSeparator.Type.SingleLine:
                            CreateSeparator(panel, separatorSingleLine);
                            break;
                        case MenuSeparator.Type.DashedLine:
                            CreateSeparator(panel, separatorDashedLine);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        var rt = panel.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        //rt.anchorMin = new Vector2(0, 0.5f);
        //rt.anchorMax = new Vector2(1, 0.5f);
        return panel;
    }

    private UiMenu CreateMenuPanel(Vector3 position, GameObject prefab)
    {
        var panel = GameObject.Instantiate(prefab);
        panel.transform.SetParent(UiManager.I.transform, false);

        var umenu = panel.GetComponent<UiMenu>();
        umenu.SetFactory(this);
        umenu.Position = position;

        //var rt = umenu.GetComponent<RectTransform>();
        //rt.anchorMin = new Vector2(0, 0.5f);
        //rt.anchorMax = new Vector2(1, 0.5f);
        return umenu;
    }

    public UiMenuLine CreateMenuItem(UiMenu menu, string text, string shortcut, GameObject prefab)
    {
        var item = GameObject.Instantiate(prefab);
        item.transform.SetParent(menu.transform, false);

        var mitem = item.GetComponent<UiMenuSimpleLine>();
        mitem.SetFactory(this);
        mitem.menuText.text = text;
        mitem.menuShortcut.text = shortcut;
        
        menu.Add(mitem);

        //var rt = item.GetComponent<RectTransform>();
        //rt.anchorMin = new Vector2(0, 0.5f);
        //rt.anchorMax = new Vector2(1, 0.5f);
        return mitem;
    }

    public UiMenuLine CreateSeparator(UiMenu menu, GameObject prefab)
    {
        var item = GameObject.Instantiate(prefab);
        item.transform.SetParent(menu.transform, false);

        var mitem = item.GetComponent<UiMenuLine>();
        mitem.SetFactory(this);
        menu.Add(mitem);

        //var rt = item.GetComponent<RectTransform>();
        //rt.anchorMin = new Vector2(0, 0.5f);
        //rt.anchorMax = new Vector2(1, 0.5f);
        return mitem;
    }
}
