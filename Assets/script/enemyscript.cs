using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class enemyscript : MonoBehaviour, IDamageable
{
    public Slider healthbar;
    battlescript bs;
    public TMP_Text shieldtext;
    public GameObject shieldobject;
    public GameObject[] intent;
    public List<movevariation> variation = new List<movevariation>();

    int shieldvalue = 0;

    void Start(){
        bs = GameObject.Find("Player").GetComponent<battlescript>();
        prepareattack();
    }
    public void damaged(int damage){
        if(shieldvalue > 0){
            if(shieldvalue < damage){
                damage -= (int)shieldvalue;
                shieldtext.text = "0";
                shieldvalue = 0;
                shieldobject.SetActive(false);
            }else{
                shieldvalue -= damage;
                shieldtext.text = shieldvalue.ToString();
                return;
            }
        }
        healthbar.value -= damage;
        if(healthbar.value <= 0){
            Destroy(this.gameObject, 0);
        }
    }

    movevariation mv;
    GameObject instantiatedIntent;

    public void prepareattack(){
        if(instantiatedIntent != null){
            Destroy(instantiatedIntent);
        }
        mv = variation[Random.Range(0, variation.Count)];
        instantiatedIntent = Instantiate(intent[mv.type], transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(0,0,0));
        instantiatedIntent.GetComponentInChildren<TMP_Text>().text = mv.value.ToString();
    }

    public void move(){
        shieldvalue = 0;
        shieldtext.text = shieldvalue.ToString();
        shieldobject.SetActive(false);
        if(mv.type == 0){
            atk();
        }else{
            def();
        }
    }

    void atk(){
        for(int i = 0; i<mv.multiplicative;i++){
            bs.damaged(mv.value);
        }
    }

    void def(){
        for(int i = 0; i<mv.multiplicative;i++){
            shieldobject.SetActive(true);
            shieldvalue += mv.value;
            shieldtext.text = shieldvalue.ToString();
        }
    }
}
