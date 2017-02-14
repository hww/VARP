using UnityEngine;

public class UiObject : MonoBehaviour
{ 
    /// <summary>
    /// Parent object in hierarchy
    /// </summary>
    protected UiMenuFactory factory;

    /// <summary>
    /// Parent object in hierarchy
    /// </summary>
    protected UiObject parent;

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

    /// <summary>
    /// Just size of rectangle
    /// </summary>
    public Vector2 RectSize
    {
        get { return RectTransform.rect.size; }
    }

    /// <summary>
    /// When we open child element where to put it
    /// </summary>
    public virtual Vector2 PreferedChildPosition { get { return PositionOnCanvas; } }

    /// <summary>
    /// Reference to the parent. For menu it can
    /// be item which open this menu
    /// </summary>
    public virtual void SetParent(UiObject parent)
    {
        this.parent = parent;
    }

    /// <summary>
    /// Reference to the factory
    /// </summary>
    public virtual void SetAnchorMinMax(Vector2 min, Vector2 max)
    {
        RectTransform.anchorMin = min;
        RectTransform.anchorMin = max;
    }


    /// <summary>
    /// Reference to the factory
    /// </summary>
    public virtual void SetFactory(UiMenuFactory factory)
    {
        this.factory = factory;
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
