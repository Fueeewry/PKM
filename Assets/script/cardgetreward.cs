using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class cardgetreward : MonoBehaviour, IPointerDownHandler
{
    public GameObject[] referenceCard;
    public GameObject cardbutton;
    public Image image;
    GameObject selectedcard;
    public void init(GameObject a){
        selectedcard = referenceCard[Random.Range(0, referenceCard.Length)];
        image.sprite = selectedcard.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        cardbutton = a;
    }
    public void OnPointerDown(PointerEventData eventData){
        battlescript.Instance.getcard(selectedcard);
        cardbutton.SetActive(false);
        transform.parent.parent.gameObject.SetActive(false);
    }
}
