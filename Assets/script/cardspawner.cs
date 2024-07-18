using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardspawner : MonoBehaviour
{
    public GameObject cardbutton;
    public GameObject[] cardlist;
    void OnEnable(){
        for(int i = 0; i < 3; i++){
            Instantiate(cardlist[Random.Range(0, cardlist.Length)], transform).GetComponent<cardgetreward>().init(cardbutton);
        }
    }
    void OnDisable(){
        while(transform.childCount > 0){
            Destroy(transform.GetChild(0));
        }
    }
}
