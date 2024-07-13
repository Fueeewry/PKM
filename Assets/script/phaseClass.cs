using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phaseClass : MonoBehaviour
{
    public List<RectTransform> cardInHandList = new List<RectTransform>();
    public List<RectTransform> drawpile = new List<RectTransform>();
    public List<RectTransform> discardpile = new List<RectTransform>();
    public Transform handTrans, drawpileTrans, discardpileTrans;

    public phaseClass phase(List<RectTransform> cardInHandList, List<RectTransform> drawpile, List<RectTransform> discardpile, Transform handTrans, Transform drawpileTrans, Transform discardpileTrans){
        this.cardInHandList = cardInHandList;
        this.drawpile = drawpile;
        this.discardpile = discardpile;
        this.handTrans = handTrans;
        this.drawpileTrans = drawpileTrans;
        this.discardpileTrans = discardpileTrans;
        return this;
    }
}
