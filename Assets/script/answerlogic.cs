using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class answerlogic : MonoBehaviour, IPointerClickHandler
{
    public bool correctanswer = false;
    questiongenerator qg;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // if(correctanswer == true){
        //     GameObject.Find("Player").enableRewards(correctanswer);
        // }
        GameObject.Find("Player").GetComponent<battlescript>().enableRewards(correctanswer);
        transform.parent.gameObject.SetActive(false);
    }

    public void init(questiongenerator a){
        qg = a;

    }
}
