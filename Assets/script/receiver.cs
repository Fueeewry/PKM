using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class receiver : MonoBehaviour, IDropHandler
{
    public string type;
    public battlescript bs;

    public void OnDrop(PointerEventData eventData){
        if(eventData.pointerDrag!=null){
            cardlogic a = eventData.pointerDrag.GetComponent<cardlogic>();
            if(type.Equals(a.type) && transform.childCount == 0 && bs.reduceEnergy(a.cost) == true){
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
}
