using System;
using UnityEngine;

namespace VARP
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;

        public static InputManager Instance
        {
            get { return instance != null ? instance : (instance = UiSingletone.FindSingletoneOfType<InputManager>()); }
        }

        // Unity will call it to deliver message
        private void OnGUI()
        {
            if (UnityEngine.Event.current.type == EventType.Repaint) return;

            switch (UnityEngine.Event.current.type)
            {
                case EventType.KeyDown:
                    OnKeyDown(UnityEngine.Event.current);
                    break;
                case EventType.KeyUp:
                    OnKeyUp(UnityEngine.Event.current);
                    break;
            }
        }

        private void OnKeyDown(UnityEngine.Event evt)
        {
            if (evt.keyCode == KeyCode.None) return;
            if (evt.keyCode >= KeyCode.RightShift && evt.keyCode <= KeyCode.RightWindows) return;

            var keyCode = Event.GetKeyCode((int)evt.keyCode);
            // ingore modifyers without key
            if (keyCode == 0) return;   


            int modifyers = 0;
            if (evt.shift) modifyers |= KeyModifyers.Shift;
            if (evt.control) modifyers |= KeyModifyers.Control;
            if (evt.alt) modifyers |= KeyModifyers.Alt;

            var keyevt = VARP.Event.MakeEvent(keyCode, modifyers);

            if (UiObject.SendOnKeyDown(keyevt))
                return;

            if (Buffer.CurentBuffer.OnKeyDown(keyevt))
                return;

        }

        private void OnKeyUp(UnityEngine.Event evt)
        {
            // nothing
        }

    }
}

public interface IOnKeyDown
{
    bool OnKeyDown(int evt);
}