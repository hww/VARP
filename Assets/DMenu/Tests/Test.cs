using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

    public GUISkin customSkin;

    private GUIStyle OnOffSlider;
    private GUIStyle OnOffSliderThumb;
    private GUIStyle MusicSlider;
    private GUIStyle MusicSliderThumb;
    private GUIStyle SoundSliderThumb;

    private GUIStyle ButtonTiny;
    private GUIStyle ButtonBig;
    private GUIStyle XButton;
    private GUIStyle TextFieldWithX;
    private GUIStyle SmallLabbel;
    private GUIStyle SliderLabel;
    private GUIStyle PopupWindow;
    private GUIStyle Dot;
    private GUIStyle NewsBox;
    private GUIStyle RadioButton;

    private void GetSkins () {

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

    private void OnGUI()
    {
        GUI.skin = customSkin;
        GetSkins();

        menuRect = GUI.Window(0, menuRect, WindowFunction, "My Window");

    }

    private void WindowFunction(int windowID)
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
