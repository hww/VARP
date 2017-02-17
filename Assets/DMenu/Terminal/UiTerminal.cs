using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using JetBrains.Annotations;
using Console = VARP.Console;
using VARP;
using VARP.Utils;

public partial class UiTerminal : UiObject, ITerminal
{

    #region Singletone

    private static UiTerminal instance;
    public static UiTerminal Instance
    {
        get { return instance!=null ? instance : (instance = UiSingletone.FindSingletoneOfType<UiTerminal>()); }
    }

    #endregion

    [System.Serializable]
    private class ColorStyle
    {
        public Color foregroundColor = Color.white;
        public Color backgroundColor = Color.black;
        public float caretBlinkRate = 1f;
        public Color caretColor = Color.red;
    }

    [Header("Setup Terminal")]
    public GameObject terminalGui;  //< to controll visibility
    public ScrollRect scrollRect;   //< scrolller
    public Transform textContainer; //< add text elements below this
    public Text promptText;         //< prompt text field
    public InputField inputField;   //< text input
    [Header("Sounds")]
    public AudioSource beepSound;   //< sound source
    public AudioClip beepClip;      //< beep clip
    [Header("Setup Terminal")]
    public GameObject textPrefab;   //< single text line

    [Header("Style")]
    [SerializeField]
    private ColorStyle style = new ColorStyle();

    private Color foregroundColor = Color.white;
    private Text curentTextLine;
    private bool setVerticalNormalizedPosition;                 //< Scroll down after add each line 
    private const float VerticalNormalizedPosition = -0.02f;    //< Use this scroll value to when scroll down

    private void Awake()
    {
        inputField.caretBlinkRate = style.caretBlinkRate;
        inputField.caretColor = style.caretColor;
        inputField.customCaretColor = true;
        Console.terminal = this;

        SetColor(Color.red);
        WriteLine(LoremIpsum.Text);
        SetColor(Color.white);
    }

    private void Update()
    {
        if (setVerticalNormalizedPosition)
        {
            setVerticalNormalizedPosition = false;
            scrollRect.verticalNormalizedPosition = VerticalNormalizedPosition;

        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            IsVisible = !IsVisible;
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inputField.isFocused)
                inputField.text = Console.OnAutocomplete(inputField.text, inputField.caretPosition);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Console.OnReadLine(inputField.text);
            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    public bool IsVisible
    {
        get { return terminalGui.activeSelf; }
        set
        {
            terminalGui.SetActive(value);
            inputField.Select();
            inputField.ActivateInputField();
        }
    }


    #region ITerminal Methods

    public void Write([NotNull] string message)
    {
        if (message == null) throw new ArgumentNullException("message");
        if (message == string.Empty) return;

        if (curentTextLine == null) curentTextLine = CreateTextObject();

        var lines = message.Split('\n');
        if (lines.Length == 1)
        {
            curentTextLine.text += message;
            scrollRect.verticalNormalizedPosition = VerticalNormalizedPosition;
        }
        else
        {
            foreach (var line in lines)
                WriteLineInternal(line);
        }
    }

    // Write text to the input field and add new line
    public void WriteLine([NotNull] string message)
    {
        if (message == null) throw new ArgumentNullException("message");
        string[] lines = message.Split('\n');
        foreach (var line in lines)
            WriteLineInternal(line);
    }

    // Write text on terminal. In this case to the 
    // prompt area
    public void WritePrompt(string prompt)
    {
        promptText.text = prompt;
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    // Produce beep sound if it is defined
    public void Beep()
    {
        if (beepClip!=null) beepSound.PlayOneShot(beepClip);
    }

    // Clear terminal screen
    public void Clear()
    {
        foreach (Transform tr in transform)
        {
            if (tr != textPrefab)
                Destroy(tr.gameObject);
        }
    }

    // Reset foreground color to defaut
    public void ResetColor()
    {
        foregroundColor = style.foregroundColor;
    }

    // Set foreground color
    public void SetColor(Color color)
    {
        foregroundColor = color;
    }

    // Read state of input field
    public bool GetInputLine(out string text, out int caretPosition)
    {
        text = inputField.text;
        caretPosition = inputField.caretPosition;
        return inputField.isFocused;
    }

    // Set state of input field
    public void SetInputLine(string text, int caretPosition, bool setFocus)
    {
        inputField.text = text;
        inputField.caretPosition = caretPosition;
        inputField.Select();
        inputField.ActivateInputField();
    }

    #endregion

    #region TextFactory

    private void WriteLineInternal([NotNull] string message)
    {
        if (message == null) throw new ArgumentNullException("message");
        if (curentTextLine == null) curentTextLine = CreateTextObject();
        curentTextLine.text += message;
        curentTextLine.color = foregroundColor;
        curentTextLine = CreateTextObject();
        scrollRect.verticalNormalizedPosition = VerticalNormalizedPosition;
    }

    /// <summary>
    /// Create new text widget. It represent single line of text
    /// </summary>
    /// <returns></returns>
    private Text CreateTextObject()
    {
        var obj = Instantiate(textPrefab);
        obj.SetActive(true);
        var tr = obj.GetComponent<RectTransform>();
        tr.SetParent(textContainer);
        return obj.GetComponent<Text>();
    }

    #endregion
}

