using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class relicgetlogic : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData){
        battlescript a = GameObject.Find("Player").GetComponent<battlescript>();
        a.getrelic();
        this.gameObject.SetActive(false);
    }
}
