using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class cardgetreward : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject[] referenceCard;
    public GameObject cardbutton;
    public Image image;
    GameObject selectedcard;
    bool canbeclicked = false;
    public void init(GameObject a){
        selectedcard = referenceCard[Random.Range(0, referenceCard.Length)];
        image.sprite = selectedcard.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        cardbutton = a;
        canbeclicked = true;
    }
    public void OnPointerDown(PointerEventData eventData){
        if(canbeclicked == true){
            battlescript.Instance.getcard(selectedcard);
            cardbutton.SetActive(false);
            transform.parent.parent.gameObject.SetActive(false);
        }
        cursorcontroller.Instance.ExitHover();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        cursorcontroller.Instance.EnterHover();
        soundcontroller.Instance.playsound(7);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        cursorcontroller.Instance.ExitHover();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        cursorcontroller.Instance.EnterHover();
        soundcontroller.Instance.playsound(7);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        cursorcontroller.Instance.ExitHover();
    }
}
