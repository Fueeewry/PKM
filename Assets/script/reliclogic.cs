using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class reliclogic : MonoBehaviour
{
    public Image image;
    public string name;
    public void setdata(Sprite sp){
        image.sprite = sp;
    }
}
