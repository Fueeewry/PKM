using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class cardgetreward : MonoBehaviour, IPointerDownHandler
{
    public GameObject referenceCard;
    public void OnPointerDown(PointerEventData eventData){
        battlescript a = GameObject.Find("Player").GetComponent<battlescript>();
        a.getcard(referenceCard);
        transform.parent.gameObject.SetActive(false);
    }
}
