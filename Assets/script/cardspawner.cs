using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardspawner : MonoBehaviour
{
    public GameObject cardbutton, card;
    List<GameObject> instantiatedcard = new List<GameObject>();
    void OnEnable(){
        instantiatedcard.RemoveAll(item => item == null);
        for(int i = 0; i < 3; i++){
            GameObject a = Instantiate(card, transform);
            a.GetComponent<cardgetreward>().init(cardbutton);
            instantiatedcard.Add(a);
        }
    }
    void OnDisable(){
        foreach(GameObject a in instantiatedcard){
            Destroy(a);
        }
    }
}
