using System;
using System.Collections;
using System.Collections.Generic;
using VARP;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public partial class UiManager : UiObject, IOnKeyDown
{

    public UiMenuFactory menuFactory;
    public UiMenuFactory menuBarFactory;

    [Header("Script Accesible")]
    private Canvas canvas;
    private UiMenu menuBar;
    private UiMenu toolBar;
    private UiMenu curentMenu;

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        //CreateTestMenu(new Vector3(100, -150, 0)); 

        var m1 = new KeyMap("Menu1", "Help2");
        m1.Define(new[] { "1" }, new MenuLineBaseSimple("Help2.1", "?I1", "Help Item1"));
        m1.Define(new[] { "2" }, new MenuLineBaseSimple("Help2.2", "?I2", "Help Item2"));
        m1.Define(new[] { "3" }, new MenuLineBaseSimple("Help2.3", "?I3", "Help Item3"));
        m1.Define(new[] { "4" }, new MenuLineBaseSimple("Help2.4", "?I4", "Help Item4"));
        m1.Define(new[] { "-3" }, new MenuSeparator(MenuSeparator.Type.Space));

       

        var m = new KeyMap("Menu1", "Help1");
        m.Define(new[] { "1" }, new MenuLineBaseSimple("Menu1.1", m1, "?I1", "Help Item1"));
        m.Define(new[] { "2" }, new MenuLineBaseSimple("Menu1.2", "help", "?I2", "Help Item2"));
        m.Define(new[] { "3" }, new MenuLineBaseSimple("Menu1.3", m1, "?I3", "Help Item3"));
        m.Define(new[] { "4" }, new MenuLineBaseSimple("Menu1.4", "?I4", "Help Item4"));
        m.Define(new[] { "5" }, new MenuLineBaseSimple("Menu1.5", "?I5", "Help Item5"));
        m.Define(new[] { "6" }, new MenuLineBaseSimple("Menu1.6", "?I6", "Help Item6"));
        curentMenu = CreateMenu(m, new Vector3(0, 0, 0), 200f);

        // m.Define(new[] { "-1" }, new MenuSeparator(MenuSeparator.Type.NoLine));
        //m.Define(new[] { "-2" }, new MenuSeparator(MenuSeparator.Type.SingleLine));
        //m.Define(new[] { "-3" }, new MenuSeparator(MenuSeparator.Type.Space));
        //m.Define(new[] { "-4" }, new MenuSeparator(MenuSeparator.Type.DashedLine));

    }

    public UiMenu CreateMenuBar(KeyMap menu, Vector3 position, float width = 0, UiObject parent = null)
    {
        return menuBarFactory.CreateMenu(menu, position, width, parent);
    }

    public UiMenu CreateMenu(KeyMap menu, UiObject parent = null)
    {
        return menuFactory.CreateMenu(menu, Vector3.zero, 200f, parent);
    }

    public UiMenu CreateMenu(KeyMap menu, Vector3 position, float width = 0, UiObject parent = null)
    {
        return menuFactory.CreateMenu(menu, position, width, parent);
    }

    public void DestoryMenu(UiObject menu)
    {
        Destroy(menu.gameObject);
    }

    private static UiManager instance;
    public static UiManager I
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UiManager>();
            return instance;
        }
    }

}



