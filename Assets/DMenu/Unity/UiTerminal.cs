using System;
using System.Collections;
using System.Collections.Generic;
using DMenu;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public partial class UiTerminal : UiObject
{
    public class Entry
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
    public Text promptText;

    private bool isVisible;

    public class MessageList
    {
        private readonly List<Entry> listOfStrings;
        private int lineIndex;

        public MessageList()
        {
            listOfStrings = new List<Entry>();
        }

        public Entry GetSelectedMessage()
        {
            if (lineIndex == 0) return null;
            if (lineIndex > listOfStrings.Count)
                lineIndex = listOfStrings.Count;
            return listOfStrings[listOfStrings.Count - lineIndex];
        }

        public string GetSelectedMessageText()
        {
            Entry ent = GetSelectedMessage();
            return ent == null ? string.Empty : ent.message;
        }


        public int LineIndex
        {
            get { return lineIndex; }
            set
            {
                lineIndex = value;
                if (lineIndex < 0)
                    lineIndex = 0;
                else if (lineIndex > listOfStrings.Count)
                    lineIndex = listOfStrings.Count;
            }
        }

        public void Add(Entry message)
        {
            listOfStrings.Add(message);
            lineIndex = 0;
        }

        public void Clear()
        {
            foreach (var entry in listOfStrings)
                if (entry.text != null) Destroy(entry.text.gameObject);

            listOfStrings.Clear();
            lineIndex = 0;
        }


    }

    private MessageList messages;
    private TheMode terminalMode;
    private UiManager uiManager;
    private void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
        // it is visible by default
        messages = new MessageList();
        isVisible = true;
        textPrefab.SetActive(false);
        inputField.onEndEdit.AddListener(delegate { OnEndEdit(inputField); });
        inputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };

        var keyMap = new KeyMap();
        keyMap.Define(Kbd.Parse("Tab"), new NativeFunction("autocomplete", Autocomplete ));
        terminalMode = new TheMode("Terminal","Simple terminal", keyMap);
    }

    private object Autocomplete(params object[] args)
    {
        return null;
    }

    private char MyValidate(char charToValidate)
    {
        //if (inputField.caretPosition<promptText.Length)
        //    charToValidate = '\0';
        if (charToValidate == '\t')
        {
            if (inputField.text.StartsWith("he"))
                inputField.text = "help";
        }
        return charToValidate;
    }

    private bool setVerticalNormalizedPosition;
    public float verticalNormalizedPosition = -0.01f;

    private void Update()
    {
        if (setVerticalNormalizedPosition)
        {
            setVerticalNormalizedPosition = false;
            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;

        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetVisible(!GetVisible());
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            messages.LineIndex++;
            Prompt(null, messages.GetSelectedMessageText());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            messages.LineIndex--;
            Prompt(null, messages.GetSelectedMessageText());
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inputField.isFocused)
            {
                if (inputField.text.StartsWith("he"))
                    inputField.text = "help";
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Log(inputField.text);
            Prompt();
        }
    }

    void OnEndEdit(InputField input)
    {
    //   if (input.text == string.Empty)
    //       return;
    //
    //   Log(input.text);
    //   Prompt();
    }

    private string defaultPrompt = ">> ";

    void Prompt(string prompt = null, string message = null)
    {
        promptText.text = prompt ?? defaultPrompt;
        inputField.text = message; 
        inputField.Select();
        inputField.ActivateInputField();
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
        messages.Add(message);
        Prompt();
        setVerticalNormalizedPosition = true;
    }

    public void LogClear()
    {
        messages.Clear();
        setVerticalNormalizedPosition = true;
    }


    private void SetPrefferedScroll()
    {
        //if (scrollRect.verticalScrollbar.isActiveAndEnabled)
        {
            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
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
        Prompt();
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

