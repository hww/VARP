using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerminal
{
    // Write text to the input field
    void Write(string message);

    // Write text to the input field and add new line
    void WriteLine(string message);

    // Write text on terminal. In this case to the 
    // prompt area
    void WritePrompt(string prompt);

    // Produce beep sound if it is defined
    void Beep();

    // Clear terminal screen
    void Clear();

    // Reset foreground color to defaut
    void ResetColor();

    // Set foreground color
    void SetColor(Color color);

}
