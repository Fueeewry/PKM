using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundcontroller : MonoBehaviour
{
    public static soundcontroller Instance {get; private set;} = null;
 
    private void Awake(){
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public AudioSource[] audi;

    public void playsound(int a){
        if(a >= 0 && a <= audi.Length){
            audi[a].Play();
        }
    }
}
