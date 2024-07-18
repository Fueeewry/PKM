using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class cardgetreward : MonoBehaviour, IPointerDownHandler
{
    public GameObject referenceCard;
    public GameObject cardbutton;
    public void init(GameObject a){
        cardbutton = a;
    }
    public void OnPointerDown(PointerEventData eventData){
        battlescript.Instance.getcard(referenceCard);
        cardbutton.SetActive(false);
        transform.parent.parent.gameObject.SetActive(false);
    }
}
