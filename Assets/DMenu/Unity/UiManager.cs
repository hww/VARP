using System;
using System.Collections;
using System.Collections.Generic;
using DMenu;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public partial class UiManager : UiObject
{

    [Header("Style")]
    public Color panelColor = Color.black;
    public ColorBlock colors;
    public Color textNormalColor = Color.white;
    public Color textHighlightedColor = Color.black;

    [Header("Prefabs")]
    public GameObject[] prefabs;
    [Header("Script Accesible")]

    private Canvas canvas;
    private List<UiMenu> menuList;

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuList = new List<UiMenu>();
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        //CreateTestMenu(new Vector3(100, -150, 0)); 

        var m = new Menu("Menu1", "?1", "Help1");
        m.Define(new[] { "1" }, new MenuItemSimple("Item1", "?I1", "Help Item1"));
        m.Define(new[] { "2" }, new MenuItemSimple("Item2", "?I2", "Help Item2"));
        m.Define(new[] { "-1" }, new MenuSeparator(MenuSeparator.Type.NoLine));
        m.Define(new[] { "3" }, new MenuItemSimple("Item3", "?I3", "Help Item3"));
        m.Define(new[] { "-2" }, new MenuSeparator(MenuSeparator.Type.SingleLine));
        m.Define(new[] { "4" }, new MenuItemSimple("Item4", "?I4", "Help Item4"));
        m.Define(new[] { "-3" }, new MenuSeparator(MenuSeparator.Type.Space));
        m.Define(new[] { "5" }, new MenuItemSimple("Item5", "?I5", "Help Item5"));
        m.Define(new[] { "-4" }, new MenuSeparator(MenuSeparator.Type.DashedLine));
        m.Define(new[] { "6" }, new MenuItemSimple("Item6", "?I6", "Help Item6"));

        var panel = CreateMenu(m, new Vector3(100, -150, 0), 200f);
    }


    public void DestoryMenu(UiMenu menu)
    {
        menuList.Remove(menu);
        Destroy(menu.gameObject);
    }

    public void OnTransformChildChanged(UiMenu menu)
    {
        foreach (var item in menuList)
        {
            if (item != null)
                item.OnTransformSiblingChanged(menu);
        }
    }

}


public partial class UiManager
{

    /// <summary>
    /// Find named prefab to isntantiate
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public GameObject GetPrefab([NotNull] string prefabName)
    {
        if (prefabName == null) throw new ArgumentNullException("prefabName");
        foreach (var prefab in prefabs)
        {
            if (prefab.name == prefabName)
                return prefab;
        }
        throw new Exception("Prefab is not found: " + prefabName);
    }

    public UiMenu CreateMenu(Menu menu, Vector3 position, float width, string panelName = null)
    {
        var panel = CreateMenuPanel(position);

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
                                CreateMenuItem(panel, mitem.Text, mitem.Shorcut, "Menu_LineSimple");
                                break;
                            case MenuItemComplex.ButtonType.Toggle:
                                CreateMenuItem(panel, mitem.Text, mitem.Shorcut, "Menu_LineToggle");
                                break;
                            case MenuItemComplex.ButtonType.Radio:
                                CreateMenuItem(panel, mitem.Text, mitem.Shorcut, "Menu_LineRadio");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                        CreateMenuItem(panel, mitem.Text, mitem.Shorcut, "Menu_LineSimple");
                }

                else if (mitem is MenuSeparator)
                {
                    switch ((mitem as MenuSeparator).type)
                    {
                        case MenuSeparator.Type.NoLine:
                            CreateSeparator(panel, "Menu_Separator_NoLine");
                            break;
                        case MenuSeparator.Type.Space:
                            CreateSeparator(panel, "Menu_Separator_Space");
                            break;
                        case MenuSeparator.Type.SingleLine:
                            CreateSeparator(panel, "Menu_Separator_SingleLine");
                            break;
                        case MenuSeparator.Type.DashedLine:
                            CreateSeparator(panel, "Menu_Separator_DashedLine");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        return panel;
    }

    private UiMenu CreateMenuPanel(Vector3 position, string prefabName = null)
    {
        var prefab = GetPrefab(prefabName ?? "Menu_Panel");
        var panel = GameObject.Instantiate(prefab);
        panel.transform.SetParent(transform, false);

        var umenu = panel.GetComponent<UiMenu>();
        umenu.UiManager = this;
        umenu.Position = position;
        return umenu;
    }

    public UiMenuLine CreateMenuItem(UiMenu menu, string text, string shortcut, string prefabName = null)
    {
        var prefab = GetPrefab(prefabName ?? "Menu_LineSimple");
        var item = GameObject.Instantiate(prefab);
        item.transform.SetParent(menu.transform, false);

        var mitem = item.GetComponent<UiMenuSimpleLine>();
        mitem.UiManager = this;
        mitem.menuText.text = text;
        mitem.menuShortcut.text = shortcut;
        //mitem.menuButton.onClick.AddListener(() => { OnClickMenuItemHandle(mitem); });
        menu.Add(mitem);

        var rt = item.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMin = new Vector2(1, 0.5f);
        return mitem;
    }

    public UiMenuLine CreateSeparator(UiMenu menu, string prefabName = null)
    {
        var prefab = GetPrefab(prefabName ?? "MenuSeparator_SingleLine");
        var item = GameObject.Instantiate(prefab);
        item.transform.SetParent(menu.transform, false);

        var mitem = item.GetComponent<UiMenuLine>();
        mitem.UiManager = this;
        menu.Add(mitem);

        var rt = item.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMin = new Vector2(1, 0.5f);
        return mitem;
    }

}
