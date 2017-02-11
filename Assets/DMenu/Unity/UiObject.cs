using UnityEngine;

public class UiObject : MonoBehaviour
{
    /// <summary>
    /// Reference to the manager
    /// </summary>
    private UiManager uiManager;

    /// <summary>
    /// Return this 2D object position
    /// </summary>
    public Vector2 Position
    {
        get { return RectTransform.anchoredPosition; }
        set { RectTransform.anchoredPosition = value; }
    }

    private RectTransform rectTransformCashed;

    /// <summary>
    /// Return rect transform with caching it
    /// </summary>
    public RectTransform RectTransform
    {
        get { return rectTransformCashed ?? (rectTransformCashed = GetComponent<RectTransform>()); }
    }

    /// <summary>
    /// Return position of this object at the canvas
    /// </summary>
    public Vector2 PositionOnCanvas
    {
        get { return UiObject.GetPositionOnCanvas(transform); }
    }

    public Vector2 RectSize
    {
        get { return RectTransform.rect.size; }
    }

    /// <summary>
    /// Reference to the manager
    /// </summary>
    public UiManager UiManager
    {
        get { return uiManager; }
        set {
            uiManager = value;
            OnSetUiMenu();
        }
    }

    protected virtual void OnSetUiMenu()
    {
    }

    /// <summary>
    /// Return position of the object at the canvas
    /// Can be done better but Unity doc a bit 
    /// confused
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Vector2 GetPositionOnCanvas(Transform transform)
    {
        var position = Vector2.zero;

        var tr = transform as RectTransform;
        while (tr != null)
        {
            if (tr.GetComponent<Canvas>() != null) break;
            position += tr.anchoredPosition;
            tr = tr.parent as RectTransform;
        }
        return position;
    }
}
