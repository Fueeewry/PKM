using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class enemyscript : MonoBehaviour, IDamageable
{
    public Slider healthbar;
    public TMP_Text shieldtext, healthtext;
    public GameObject shieldobject;
    public GameObject[] intent;
    public List<movevariation> variation = new List<movevariation>();
    Animator anim;

    int shieldvalue = 0;

    void Start(){
        //bs = GameObject.Find("Player").GetComponent<battlescript>();
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        anim = GetComponent<Animator>();
        if(anim!=null){
            anim.SetFloat("startrandom", Random.Range(0.2f, 0.5f));
            anim.speed = Random.Range(1f, 2f);
        }
        prepareattack();
    }
    public void damaged(int damage){
        if(shieldvalue > 0){
            if(shieldvalue > damage){
                shieldvalue -= damage;
                shieldtext.text = shieldvalue.ToString();
            }else{
                damage -= shieldvalue;
                shieldtext.text = "0";
                shieldvalue = 0;
                shieldobject.SetActive(false);
                return;
            }
        }
        healthbar.value -= damage;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        battlescript.Instance.showEffect(damage, transform.position);

        if(healthbar.value <= 0){
            if(anim!=null){
                anim.Play("enemydead");
            }
            Destroy(this.gameObject, 0.4f);
        }
    }

    movevariation mv;
    GameObject instantiatedIntent;

    public void prepareattack(){
        if(instantiatedIntent != null){
            battlescript.Instance.killedanenemy();
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
            battlescript.Instance.damaged(mv.value);
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
