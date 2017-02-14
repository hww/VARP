using System;
using System.Collections;
using System.Collections.Generic;
using DMenu;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Menu = DMenu.Menu;

[RequireComponent(typeof(Canvas))]
public partial class UiManager : UiObject
{
    public UiMenuFactory menuFactory;
    public UiMenuFactory menuBarFactory;

    [Header("Script Accesible")]

    private Canvas canvas;
    private List<UiObject> menuList;

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuList = new List<UiObject>();
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        //CreateTestMenu(new Vector3(100, -150, 0)); 

        var m1 = new Menu("Menu1", "?1", "Help1");
        m1.Define(new[] { "1" }, new MenuItemSimple("Item1", "?I1", "Help Item1"));
        m1.Define(new[] { "2" }, new MenuItemSimple("Item2", "?I2", "Help Item2"));
        m1.Define(new[] { "3" }, new MenuItemSimple("Item3", "?I3", "Help Item3"));
        m1.Define(new[] { "4" }, new MenuItemSimple("Item4", "?I4", "Help Item4"));
        m1.Define(new[] { "-3" }, new MenuSeparator(MenuSeparator.Type.Space));

        var panel1 = CreateMenuBar(m1, new Vector3(0, 0, 0));

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
        var panel2 = CreateMenu(m, new Vector3(0, 0, 0), 200f);
    }

    private Menu testMenu;
    public UiMenu CreateTestMenu(Vector3 position)
    {
        if (testMenu == null)
        {
            testMenu = new Menu("Menu2", "?1", "Help2");
            testMenu.Define(new[] { "1" }, new MenuItemSimple("Item1", "?I1", "Help Item1"));
            testMenu.Define(new[] { "2" }, new MenuItemSimple("Item2", "?I2", "Help Item2"));
            testMenu.Define(new[] { "-1" }, new MenuSeparator(MenuSeparator.Type.NoLine));
            testMenu.Define(new[] { "3" }, new MenuItemSimple("Item3", "?I3", "Help Item3"));
            testMenu.Define(new[] { "-2" }, new MenuSeparator(MenuSeparator.Type.SingleLine));
            testMenu.Define(new[] { "4" }, new MenuItemSimple("Item4", "?I4", "Help Item4"));
            testMenu.Define(new[] { "-3" }, new MenuSeparator(MenuSeparator.Type.Space));
            testMenu.Define(new[] { "5" }, new MenuItemSimple("Item5", "?I5", "Help Item5"));
            testMenu.Define(new[] { "-4" }, new MenuSeparator(MenuSeparator.Type.DashedLine));
            testMenu.Define(new[] { "6" }, new MenuItemSimple("Item6", "?I6", "Help Item6"));
        }

        return CreateMenu(testMenu, position, 200f);
    }

    public UiMenu CreateMenuBar(Menu menu, Vector3 position, float width = 0)
    {
        return menuBarFactory.CreateMenu(menu, position, width);
    }

    public UiMenu CreateMenu(Menu menu, Vector3 position, float width = 0)
    {
        return menuFactory.CreateMenu(menu, position, width);
    }

    public void DestoryMenu(UiObject menu)
    {
        menuList.Remove(menu);
        Destroy(menu.gameObject);
    }

    private static UiManager instance;
    public static UiManager I
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<UiManager>();
            return instance;
        }
    }

}



