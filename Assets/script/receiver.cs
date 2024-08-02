using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class receiver : MonoBehaviour, IDropHandler
{
    public cardtype type;
    public battlescript bs;
    public bool canget = true;
    public GameObject ss;

    public void OnDrop(PointerEventData eventData){
        if(eventData.pointerDrag!=null){
            cardlogic a = eventData.pointerDrag.GetComponent<cardlogic>();
            if(battlescript.Instance.checkInteractionOnGoing() == false && a.type == type && transform.childCount == 0 && bs.reduceEnergy(a.cost) == true && canget == true){
                a.turnRaycast(false);
                eventData.pointerDrag.transform.SetParent(transform, false);
                bs.removeCard(eventData.pointerDrag.GetComponent<RectTransform>());
                a.disable();
                switch(a.type){
                    case cardtype.function:
                        bs.addFunction(a.function);
                        break;
                    case cardtype.variable:
                        bs.addVariable(a.variable);
                        break;
                    case cardtype.effect:
                        bs.addEffect(a.effect);
                        break;
            }
            }
        }
    }

    public void changestate(bool a){
        canget = a;
        ss.SetActive(!a);
    }
}
