using System;
using System.Runtime.Remoting;
using UnityEngine;

public abstract partial class UiObject : MonoBehaviour
{
    #region Transform & Position

    private RectTransform rectTransformCashed;

    /// <summary>
    /// Return rect transform with caching it
    /// </summary>
    public RectTransform RectTransform
    {
        get { return rectTransformCashed ?? (rectTransformCashed = GetComponent<RectTransform>()); }
    }

    /// <summary>
    /// Return this 2D object position
    /// </summary>
    public Vector2 Position
    {
        get { return RectTransform.anchoredPosition; }
        set { RectTransform.anchoredPosition = value; }
    }

    /// <summary>
    /// Return position of this object at the canvas
    /// </summary>
    public Vector2 PositionOnCanvas
    {
        get { return GetPositionOnCanvas(transform); }
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

    #endregion

    /// <summary>
    /// When we open child element where to put it? This value for it.
    /// </summary>
    public virtual Vector2 ChildPosition { get { return PositionOnCanvas; } }

}

public partial class UiObject
{
    private static UiObject curentFocusObject;  // curentrly focused object
    
    protected UiObject previousFocusObject;     // this is the object was focused before open this menu


    /// <summary>
    /// curently focused object
    /// </summary>
    public static UiObject CurentFocusObject
    {
        get { return curentFocusObject; }
    }

    /// <summary>
    /// Make this object in focus
    /// </summary>
    public virtual bool IsFocused
    {
        get { return curentFocusObject == this; }
    }

    /// <summary>
    /// Make this object icused 
    /// </summary>
    public virtual void Focus()
    {
        if (curentFocusObject == this) return;
        previousFocusObject = curentFocusObject;
        curentFocusObject = this;
        RequestRefresh();
        Debug.Log(curentFocusObject);
    }

    /// <summary>
    /// Defocus this object
    /// </summary>
    public virtual void Defocus()
    {
        if (curentFocusObject == this)
            curentFocusObject = previousFocusObject;
        RequestRefresh();
    }
}

public partial class UiObject
{
    /// <summary>
    /// Factory is the creator of this object 
    /// it also has the style data for this object
    /// </summary>
    protected UiMenuFactory factory;

    /// <summary>
    /// Factory is the creator of this object 
    /// it also has the style data for this object
    /// </summary>
    public virtual void SetFactory(UiMenuFactory factory)
    {
        this.factory = factory;
        RequestRefresh();
    }
}

public partial class UiObject
{
    /// <summary>
    /// Reference to the parent. 
    /// For menu item it point to menu panel
    /// For menu panel it point to menu item which open this menu
    /// </summary>
    public UiObject parentFocus;

    /// <summary>
    /// In case when this object is member of menu. Can user select it?
    /// </summary>
    public virtual bool IsSelectable { get { return false; } }

    private bool isSelected;

    /// <summary>
    /// Make this object looks as selected
    /// </summary>
    public virtual bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value; RequestRefresh(); }
    }


    /// <summary>
    /// Close element
    /// </summary>
    public virtual void Close()
    {
        Defocus();
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// Checking  if the destroy was done correctly
    /// </summary>
    void OnDestroy()
    {
        Defocus();
    }
}

public partial class UiObject : IOnKeyDown
{
    /// <summary>
    /// By default user interface object ignores the key event
    /// </summary>
    public virtual bool OnKeyDown(int kevt)
    {
        return false;
    }

    public static bool SendOnKeyDown(int evt)
    {
        if (CurentFocusObject == null) return false;

        var cfo = CurentFocusObject;
        while (cfo != null)
        {
            if (cfo.OnKeyDown(evt))
                return true;

            cfo = cfo.parentFocus;
        }
        return false;
    }
}

public partial class UiObject
{
    protected bool refresh;

    /// <summary>
    /// To refresh the object on screen
    /// </summary>
    public virtual void Refresh()
    {
        refresh = false;
    }

    public void RequestRefresh()
    {
        refresh = true;
    }
}