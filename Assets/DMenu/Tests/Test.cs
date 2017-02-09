using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

    public GUISkin customSkin;

    GUIStyle OnOffSlider;
    GUIStyle OnOffSliderThumb;
    GUIStyle MusicSlider;
    GUIStyle MusicSliderThumb;
    GUIStyle SoundSliderThumb;

    GUIStyle ButtonTiny;
    GUIStyle ButtonBig;
    GUIStyle XButton;
    GUIStyle TextFieldWithX;
    GUIStyle SmallLabbel;
    GUIStyle SliderLabel;
    GUIStyle PopupWindow;
    GUIStyle Dot;
    GUIStyle NewsBox;
    GUIStyle RadioButton;

    void GetSkins () {

        OnOffSlider = GUI.skin.GetStyle("OnOffSlider");
        OnOffSliderThumb = GUI.skin.GetStyle("OnOffSliderThumb");
        MusicSlider = GUI.skin.GetStyle("musicSlider");
        MusicSliderThumb = GUI.skin.GetStyle("musicSliderThumb");
        SoundSliderThumb = GUI.skin.GetStyle("soundSliderThumb");

        ButtonTiny = GUI.skin.GetStyle("ButtonTiny");
        ButtonBig = GUI.skin.GetStyle("ButtonBig");
        XButton = GUI.skin.GetStyle("XButton");
        TextFieldWithX = GUI.skin.GetStyle("TextFieldWithX");

        SmallLabbel = GUI.skin.GetStyle("SmallLabbel");
        SliderLabel = GUI.skin.GetStyle("SliderLabel");
        PopupWindow = GUI.skin.GetStyle("PopupWindow");
        Dot = GUI.skin.GetStyle("Dot");
        NewsBox = GUI.skin.GetStyle("NewsBox");
        RadioButton = GUI.skin.GetStyle("RadioButton");
    }

    public Rect menuRect = new Rect(0, 0, 250, 250);
    void OnGUI()
    {
        GUI.skin = customSkin;
        GetSkins();

        menuRect = GUI.Window(0, menuRect, WindowFunction, "My Window");

    }

    void WindowFunction(int windowID)
    {

        //using (var verticalScope = new GUILayout.AreaScope(rectangle))
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Press Me 1"))
                Debug.Log("Hello!");
            if (GUILayout.Button("Press Me 2"))
                Debug.Log("Hello!");
            if (GUILayout.Button("Press Me 3"))
                Debug.Log("Hello!");
            GUILayout.EndVertical();
        }
    }
}
