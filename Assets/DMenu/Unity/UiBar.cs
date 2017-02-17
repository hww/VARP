using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBar : UiObject
{
    public bool isMenuBar;
    private Image image;
    private List<UiMenuLine> menuLines;
    private UiObject submenu;


    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        image = GetComponent<Image>();
        menuLines = new List<UiMenuLine>();
    }

    /// <summary>
    /// Reference to the parent. For menu it will be
    /// item used to open it. (or maybe anchor)
    /// </summary>
    public override void SetParent(UiObject parent)
    {
        this.parent = parent;
        if (parent == null) return;
        var line = parent.GetComponent<UiMenuLine>();
        if (line == null) return;

        OnRectTransformDimensionsChange();
    }

    /// <summary>
    /// Reference to the parent
    /// </summary>
    public override void SetFactory(UiMenuFactory factory)
    {
        this.factory = factory;
        image.color = factory.panelColor;
    }

    public void Add(UiMenuLine line)
    {
        menuLines.Add(line);


        var button = line.GetComponent<UiMenuSimpleLine>();
        if (button != null)
            button.menuButton.onClick.AddListener(() => { OnClickMenuItemHandle(line); });
    }

    public void Remove(UiMenuLine line)
    {
        menuLines.Remove(line);
    }

    private void OnClickMenuItemHandle(UiObject line)
    {
        if (submenu != null)
            UiManager.I.DestoryMenu(submenu);

        Vector3 pos = line.PreferedChildPosition;
        submenu = UiManager.I.CreateTestMenu(pos);
        submenu.SetParent(line);
    }

    // ReSharper disable once UnusedMember.Local
    private void OnDestroy()
    {
        if (submenu != null)
            Destroy(submenu.gameObject);
    }

    // ReSharper disable once UnusedMember.Local
    private void OnRectTransformDimensionsChange()
    {
        if (isMenuBar)
        {
            var canvasSize = UiManager.I.RectTransform.sizeDelta;
            RectTransform.sizeDelta = new Vector2(canvasSize.x, RectTransform.sizeDelta.y);
        }
        else
        {
            if (parent != null)
                RectTransform.anchoredPosition = GetFitPosition(parent);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Vector2 GetFitPosition(UiObject parent)
    {
        if (parent == null)
            return Vector2.zero;

        // anchor point
        var canvasSize = UiManager.I.RectTransform.rect.size;
        // submenu size
        var thisRect = RectTransform.rect;
        var thisPos = parent.PreferedChildPosition;
        // There is interesting phenomen. position of object is in top left 
        thisRect.position = new Vector2(thisPos.x, thisPos.y - thisRect.height);

        if (thisRect.Contains(new Vector2(canvasSize.x, thisRect.y)))
        {
            thisRect.position = new Vector2(canvasSize.x - thisRect.width, thisRect.position.y);

            var parentRect = parent.RectTransform.rect;
            var parentPos = parent.PositionOnCanvas;
            // There is interesting phenomen. position of object is in top left 
            parentRect.position = new Vector2(parentPos.x, parentPos.y - parentRect.height);

            if (thisRect.Overlaps(parentRect))
            {


                thisRect.position = new Vector2(thisRect.x, parentRect.yMin - thisRect.height);
                if (thisRect.Contains(new Vector2(thisRect.x, -canvasSize.y)))
                {
                    thisRect.position = new Vector2(thisRect.x, parentRect.yMax);
                }
            }

        }

        thisRect.position = new Vector2(thisRect.position.x, thisRect.position.y + thisRect.height);
        return thisRect.position;

        /*       Rect(anchor.x, anchor.y, submenusize.x, submenusize.y);
       
               // get the canvas size
               // top left corner of the canvas is 0,0
               var canvasSize = UiManager.I.RectTransform.sizeDelta;
               
               // get menu item size
               // pos +--------------------------+
               //     |                          |
               //     +--------------------------+ max
               var linepos = parent.PositionOnCanvas;
               var linesize = parent.RectTransform.rect.size;
               var linemax = linepos + linesize;
       
               var x = 0f;
               var y = 0f;
               if (submenumax.x < canvasSize.x)
               {
                   x = achor.x;
               }
               else
               {
                   x = canvasSize.x - submenusize.x;
               }
       
               if (newRect.Overlaps(parent.RectTransform.rect))
               {
                   
               }
               else
               {
                   return new Vector2(x, achor.y);
               }*/

    }
}
