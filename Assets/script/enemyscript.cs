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

    int shieldvalue = 0, stunned, weakentill = 0;

    float weakenvalue = 1;

    public float intentheight = 1.5f;
    public int type = 0;

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
    public void damaged(int damage, GameObject effect){
        if(effect != null){
            Instantiate(effect, transform.position, Quaternion.Euler(0,0,0));
        }
        if(shieldvalue > 0){
            if(shieldvalue > damage){
                shieldvalue -= damage;
                shieldtext.text = shieldvalue.ToString();
            }else{
                damage -= shieldvalue;
                shieldtext.text = "0";
                shieldvalue = 0;
                shieldobject.SetActive(false);
            }
        }
        healthbar.value -= damage;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        battlescript.Instance.showEffect(damage, transform.position);

        if(anim!=null){
            switch(type){
                case 0:
                    anim.SetFloat("enemyhit", Random.Range(0.2f, 0.5f));
                    anim.speed = Random.Range(1f, 2f);
                    break;
                case 1:
                    anim.SetFloat("bosshit", Random.Range(0.2f, 0.5f));
                    anim.speed = Random.Range(1f, 2f);
                    break;
                case 2:
                    anim.SetFloat("dronehit", Random.Range(0.2f, 0.5f));
                    anim.speed = Random.Range(1f, 2f);
                    break;
            }
        }

        if(healthbar.value <= 0){
            battlescript.Instance.killedanenemy();
            enemyspawner.Instance.enemyInstantiatedList.Remove(this);
            if(anim!=null){
                anim.Play("enemydead");
            }
            if(instantiatedIntent != null){
                Destroy(instantiatedIntent);
            }
            Destroy(this.gameObject, 0.4f);
        }
    }
    public int stealhealth(int damage){
        int orihealth = (int)healthbar.value;
        healthbar.value -= damage;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        battlescript.Instance.showEffect(damage, transform.position);

        if(healthbar.value <= 0){
            battlescript.Instance.killedanenemy();
            enemyspawner.Instance.enemyInstantiatedList.Remove(this);
            if(anim!=null){
                anim.Play("enemydead");
            }
            if(instantiatedIntent != null){
                Destroy(instantiatedIntent);
            }
            Destroy(this.gameObject, 0.4f);
        }

        return orihealth - (int)healthbar.value;
    }

    public void stunfor(int value){
        stunned += value;
    }
    public void reducedamageby(float a, int b){
        weakentill = b;
        weakenvalue = a;
    }

    movevariation mv;
    GameObject instantiatedIntent;

    public bool checkstillalive(){
        if(gameObject == null){
            return false;
        }
        return true;
    }

    public void prepareattack(){
        if(stunned > 0){
            stunned--;
            return;
        }
        if(instantiatedIntent != null){
            Destroy(instantiatedIntent);
        }
        mv = variation[Random.Range(0, variation.Count)];
        instantiatedIntent = Instantiate(intent[mv.type], transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(0,0,0));
        instantiatedIntent.GetComponentInChildren<TMP_Text>().text = (mv.value * weakenvalue).ToString();
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
        soundcontroller.Instance.playsound(8);
        for(int i = 0; i<mv.multiplicative;i++){
            Debug.Log((int)(mv.value  * weakenvalue));
            battlescript.Instance.damaged((int)(mv.value  * weakenvalue));
        }
        if(weakentill <= 0){
            weakenvalue = 1;
        }else{
            weakentill--;
        }
    }

    void def(){
        soundcontroller.Instance.playsound(0);
        for(int i = 0; i<mv.multiplicative;i++){
            shieldobject.SetActive(true);
            shieldvalue += mv.value;
            shieldtext.text = shieldvalue.ToString();
        }
    }
}
