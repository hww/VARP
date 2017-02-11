using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UiTerminal : UiObject
{
    private class Entry
    {
        public string message;
        public Color color;
        public readonly Text text;

        public Entry(string message, Color color, Text text)
        {
            this.color = color;
            this.message = message;
            this.text = text;

            text.text = message;
            text.color = color;
        }
    };

    private static UiTerminal instance;

    public static UiTerminal I
    {
        get { return instance ?? (instance = FindObjectOfType<UiTerminal>()); }
    }

    public GameObject terminalGui;
    public GameObject textPrefab;
    public InputField inputField;
    public ScrollRect scrollRect;

    private bool isVisible;


    private List<Entry> listOfStrings;

    private void Start()
    {
        listOfStrings = new List<Entry>();
        UiManager = FindObjectOfType<UiManager>();
        // it is visible by default
        isVisible = true;
        textPrefab.SetActive(false);
        inputField.onEndEdit.AddListener(delegate { OnEndEdit(inputField); });
        Log("11111");
        LogError("22222");
        LogWarning("33333");
        Log("44444");
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetVisible(!GetVisible());
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (lineIndex < listOfStrings.Count) lineIndex++;
            Prompt(GetMessageAt(lineIndex));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (lineIndex > 0) lineIndex--;
            Prompt(GetMessageAt(lineIndex));
        }
    }

    string GetMessageAt(int index)
    {
        if (index == 0) return string.Empty;
        if (index > listOfStrings.Count) index = listOfStrings.Count;
        return listOfStrings[listOfStrings.Count-index].message;
    }

    void OnEndEdit(InputField input)
    {
        Log(input.text);
        Prompt(string.Empty);
    }

    void Prompt(string message)
    {
        inputField.text = message;
        if (isVisible)
            inputField.Select();
    }
}

public partial class UiTerminal
{

    public void Log(string message)
    {
        Add(new Entry(message, Color.white, CreateTextObject()));

    }

    public void LogError(string message)
    {
        Add(new Entry(message, Color.red, CreateTextObject()));

    }


    public void LogWarning(string message)
    {
        Add(new Entry(message, Color.yellow, CreateTextObject()));
        
    }

    private void Add(Entry message)
    {
        listOfStrings.Add(message);
        SetPrefferedScroll();
        lineIndex = 0;
        SetPrefferedScroll();
    }

    public void LogClear()
    {
        foreach (var entry in listOfStrings)
            if (entry.text != null) Destroy(entry.text.gameObject);

        listOfStrings.Clear();
        SetPrefferedScroll();
        lineIndex = 0;
    }

    private int lineIndex;
    private void SetPrefferedScroll()
    {
        //if (scrollRect.verticalScrollbar.isActiveAndEnabled)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            //Canvas.ForceUpdateCanvases();
        }
    }

}


/// <summary>
    /// Control visibility
    /// </summary>
    public partial class UiTerminal
{

    private void SetVisible(bool state)
    {
        isVisible = state;
        terminalGui.SetActive(state);
        if (state)
            inputField.Select();
    }

    private bool GetVisible()
    {
        return isVisible;
    }
}

/// <summary>
/// The objects factory
/// </summary>
public partial class UiTerminal
{ 

    /// <summary>
    /// Create new text widget
    /// </summary>
    /// <returns></returns>
    private Text CreateTextObject()
    {
        var obj = Instantiate(textPrefab);
        obj.SetActive(true);
        var tr = obj.GetComponent<RectTransform>();
        tr.SetParent(textPrefab.transform.parent);
        return obj.GetComponent<Text>();
    }

}

