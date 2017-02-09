using System;
using System.Collections;
using System.Collections.Generic;
using DMenu;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UiMenu : UiObject {

    [Header("Prefabs")]
    public GameObject[] prefabs;
    [Header("Script Accesible")]

    private Canvas canvas;
    private List<UiMenuPanel> menuList;

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuList = new List<UiMenuPanel>();
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

        CreateMenu(m, new Vector3(100, -150, 0));
    }

    public UiMenuPanel CreateTestMenu(Vector3 position)
    {
        var menu = CreateEmptyMenu(position);
        menu.CreateItem(this, "Menu 1", "?1");
        menu.CreateItem(this, "Menu 2", "?2");
        menu.CreateItem(this, "Menu 3", "?3");
        menu.CreateItem(this, "Menu 4", "?4");
        menu.CreateItem(this, "Menu 5", "?5");
        menuList.Add(menu);
        return menu;
    }

    public UiMenuPanel CreateMenu(Menu menu, Vector3 position, string panelName = null)
    {
        var umenu = CreateEmptyMenu(position);
        foreach (var item in menu.items)
        {
            var mitem = item.value as MenuItem;
            if (mitem != null)
            {
                if (mitem is MenuItemSimple)
                {
                    umenu.CreateItem(this, mitem.Text, mitem.Shorcut);
                }
                else if (mitem is MenuItemComplex)
                {
                    
                }
                else if (mitem is MenuSeparator)
                {
                    switch ((mitem as MenuSeparator).type)
                    {
                        case MenuSeparator.Type.NoLine:
                            umenu.CreateSeparator(this, "MenuSeparator_NoLine");
                            break;
                        case MenuSeparator.Type.Space:
                            umenu.CreateSeparator(this, "MenuSeparator_Space");
                            break;
                        case MenuSeparator.Type.SingleLine:
                            umenu.CreateSeparator(this, "MenuSeparator_SingleLine");
                            break;
                        case MenuSeparator.Type.DashedLine:
                            umenu.CreateSeparator(this, "MenuSeparator_DashedLine");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }
            }

        }

        return umenu;
    }
    public UiMenuPanel CreateEmptyMenu(Vector3 position, string panelName = null)
    {
        var panePrefab = GetPrefab(panelName ?? "Panel");
        var panel = GameObject.Instantiate(panePrefab);
        var umenu = panel.GetComponent<UiMenuPanel>();
        umenu.RectTransform.SetParent(transform, false);
        umenu.Position = position;
        umenu.uiMenu = this;
        return umenu;
    }

    public void DestoryMenu(UiMenuPanel menuPanel)
    {
        menuList.Remove(menuPanel);
        Destroy(menuPanel.gameObject);
    }

    public void OnTransformChildChanged(UiMenuPanel menuPanel)
    {
        foreach (var item in menuList)
        {
            if (item != null)
                item.OnTransformSiblingChanged(menuPanel);
        }
    }

    /// <summary>
    /// Find named prefab to isntantiate
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetPrefab([NotNull] string name)
    {
        if (name == null) throw new ArgumentNullException("name");
        foreach (var prefab in prefabs)
        {
            if (prefab.name == name)
                return prefab;
        }
        throw new Exception("Prefab is not found: " + name);
    }
}
