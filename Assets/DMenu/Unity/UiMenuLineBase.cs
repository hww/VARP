using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VARP;


public class UiMenuLineBase : UiObject, IOnKeyDown
{
    [Header("UiMenuLineBase")]
    public Vector2 childAnchor;
    private Vector2 childAnchorOnCanves;

    /// <summary>
    /// When we open child element where the best place to put it 
    /// </summary>
    public override Vector2 ChildPosition
    {
        get
        {
            childAnchorOnCanves.x = RectTransform.rect.size.x * childAnchor.x;
            childAnchorOnCanves.y = -RectTransform.rect.size.y * childAnchor.y;
            return PositionOnCanvas + childAnchorOnCanves;
        }
    }

    #region MenuLine

    private MenuLineBase menuLineBase;

    /// <summary>
    /// Definition of the menu line. Contains: text, shortcut, bindings
    /// </summary>
    public virtual MenuLineBase MenuLineBase
    {
        get { return menuLineBase; }
        set {
            menuLineBase = value;
            name = menuLineBase.Text;
            RequestRefresh();
        }
    }

    #endregion




}
