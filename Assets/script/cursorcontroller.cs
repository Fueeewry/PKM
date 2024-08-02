using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursorcontroller : MonoBehaviour
{
    public Texture2D cursorTexture, cursorTextureClick;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public static cursorcontroller Instance {get; private set;} = null;
 
    private void Awake(){
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void EnterHover()
    {
        Cursor.SetCursor(cursorTextureClick, hotSpot, cursorMode);
    }

    public void ExitHover()
    {
        Debug.Log("tesssst");
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}
