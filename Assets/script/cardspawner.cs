using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardspawner : MonoBehaviour
{
    public GameObject[] cardlist;
    void OnEnable(){
        for(int i = 0; i < 3; i++){
            Instantiate(cardlist[Random.Range(0, cardlist.Length)], transform);
        }
    }
}
