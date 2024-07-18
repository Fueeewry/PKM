using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class cardlogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public cardtype type;
    public string function, effect;
    public bool upgraded = false;
    public int variable = 0, cost = 1;
    Transform child;
    int childOrder;
    LineRenderer line;
    bool dragging = false, inside = false;
    Animator anim;
    public Image image;
    public TMP_Text titletxt;
    
    void Start(){
        child = transform.GetChild(0);
        line = transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        anim = this.gameObject.GetComponent<Animator>();
    }

    public void OnBeginDrag(PointerEventData eventData){
        line.enabled = true;
        transform.SetSiblingIndex(transform.parent.childCount);
    }

    public void OnDrag(PointerEventData eventData){
        dragging = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnEndDrag(PointerEventData eventData){
        dragging = false;
        disable();
        //StartCoroutine(rotate1());
    }

    public void OnPointerDown(PointerEventData eventData){
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        StopAllCoroutines();
        anim.SetTrigger("Highlighted");
        inside = true;
        childOrder = transform.GetSiblingIndex();
        transform.SetSiblingIndex(transform.parent.childCount);
        //child.localRotation = Quaternion.Euler(0,0,-transform.eulerAngles.z);
        //StartCoroutine(rotate());
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        inside = false;
        if(dragging == false){
            disable();
            //StartCoroutine(rotate1());
        }
    }

    public void disable(){
        StopAllCoroutines();
        line.enabled = false;
        anim.SetTrigger("Disabled");
        transform.SetSiblingIndex(childOrder);
        //child.localRotation = Quaternion.Euler(0,0,0);
    }
    public void turnRaycast(bool a){
        transform.rotation = Quaternion.Euler(0,0,0);
        transform.localPosition = new Vector3(0,0,0);
        image.raycastTarget = a;
    }

    IEnumerator rotate(){
        image.raycastTarget = false;
        float rot = transform.eulerAngles.z;
        for(float i = 0; i<1; i+= 0.1f){
            child.localRotation = Quaternion.Euler(0,0,-rot * i);
            yield return new WaitForSeconds(0.02f);
        }
        image.raycastTarget = true;
    }
    IEnumerator rotate1(){
        image.raycastTarget = false;
        for(float i = child.eulerAngles.z; i>0; i-= 0.2f){
            child.localRotation = Quaternion.Euler(0,0,i);
            yield return new WaitForSeconds(0.02f);
        }
        image.raycastTarget = true;
    }

    public void upgrade(){
        upgraded = true;
        titletxt.color = new Color32(50, 205, 50, 1);
    }
}
