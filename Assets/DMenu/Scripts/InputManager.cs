using System;
using System.Collections;
using System.Collections.Generic;
using DMenu;
using UnityEngine;
using Event = UnityEngine.Event;

public class InputManager : MonoBehaviour {


    /// <summary>
    /// Unity will call it to deliver message
    /// </summary>
    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint) return;

        switch (Event.current.type)
        {
            case EventType.MouseDown:
                break;
            case EventType.MouseUp:
                break;
            case EventType.MouseMove:
                break;
            case EventType.MouseDrag:
                break;
            case EventType.KeyDown:
                OnKeyDown(Event.current);
                break;
            case EventType.KeyUp:
                OnKeyUp(Event.current);
                break;
            case EventType.ScrollWheel:
                break;
            case EventType.Repaint:
                break;
            case EventType.Layout:
                break;
            case EventType.DragUpdated:
                break;
            case EventType.DragPerform:
                break;
            case EventType.DragExited:
                break;
            case EventType.Ignore:
                break;
            case EventType.Used:
                break;
            case EventType.ValidateCommand:
                break;
            case EventType.ExecuteCommand:
                break;
            case EventType.ContextClick:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static readonly int[] InputBuffer = new int[10000];
    private static int inputPosition = 0;
    private static TheMode fundamentalMode;

    public static TheMode FundamentalMode
    {
        get
        {
            if (fundamentalMode != null) return fundamentalMode;
            var keyMap = new KeyMap("global");

            fundamentalMode = new TheMode("fundamental", null, keyMap);
            return fundamentalMode;
        }
    }

    private void OnKeyDown(Event evt)
    {
        if (evt.keyCode == KeyCode.None) return;

        int modifyers = 0;
        if ((evt.modifiers & EventModifiers.Shift) != 0) modifyers |= DMenu.Event.Shift;
        if ((evt.modifiers & EventModifiers.Control) != 0) modifyers |= DMenu.Event.Control;
        if ((evt.modifiers & EventModifiers.Alt) != 0) modifyers |= DMenu.Event.Alt;
        if ((evt.modifiers & EventModifiers.CapsLock) != 0) modifyers |= DMenu.Event.Shift;
        if ((evt.modifiers & EventModifiers.Command) != 0) modifyers |= DMenu.Event.Meta;
        if ((evt.modifiers & EventModifiers.FunctionKey) != 0) modifyers |= DMenu.Event.Super;
        if ((evt.modifiers & EventModifiers.Numeric) != 0) modifyers |= DMenu.Event.Hyper;

        if (inputPosition < InputBuffer.Length)
            InputBuffer[inputPosition++] = DMenu.Event.MakeEvent((int)evt.keyCode, modifyers);

        string s = "";
        for (int i = 0; i < inputPosition; i++)
        {
            s += DMenu.Event.GetName(InputBuffer[i]);
        }
        Debug.Log(s);
    }

    private void OnKeyUp(Event evt)
    {
        //Debug.Log("Current detected event: " + evt);
    }
}
