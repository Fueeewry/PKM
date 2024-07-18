using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restsitelogic : MonoBehaviour
{
    public void heal(){
        battlescript.Instance.gethealrestsite();
        gameObject.SetActive(false);
    }
    public void upgrade(){
        battlescript.Instance.getupgrade();
    }
    public void changeSceneRest(){
        //open map
    }
}
