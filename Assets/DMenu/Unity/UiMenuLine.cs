using UnityEngine;
using UnityEngine.UI;


public class UiMenuLine : UiObject
{
    public UiMenu uiMenu;
    public Vector2 childAnchor;
    public Vector2 childAnchorOnCanves;


    /// <summary>
    /// When we open child element where to put it
    /// </summary>
    public override Vector2 PreferedChildPosition
    {
        get
        {
            childAnchorOnCanves.x = RectSize.x * childAnchor.x;
            childAnchorOnCanves.y = -RectSize.y * childAnchor.y;
            return PositionOnCanvas + childAnchorOnCanves;
        }
    }

}

