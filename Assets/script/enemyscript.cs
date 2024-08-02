using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class enemyscript : MonoBehaviour, IDamageable
{
    public Slider healthbar;
    public TMP_Text shieldtext, healthtext;
    public GameObject shieldobject, poisoneffect, poisonstack;
    public GameObject[] intent;
    public List<movevariation> variation = new List<movevariation>();
    public Animator anim;
    public Transform debufftrans;

    int shieldvalue = 0, stunned, weakentill = 0, poison = 0;

    float weakenvalue = 1;

    public float intentheight = 1.5f;
    public int type = 0;

    void Start(){
        //bs = GameObject.Find("Player").GetComponent<battlescript>();
        healthbar.maxValue += (battlescript.Instance.levelstage - 1) * 6;
        healthbar.value += healthbar.maxValue;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        if(anim == null){
            anim = GetComponent<Animator>();
        }
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
                    anim.Play("enemyhit");
                    anim.speed = Random.Range(1f, 2f);
                    break;
                case 1:
                    anim.Play("bosshit");
                    anim.speed = Random.Range(1f, 2f);
                    break;
                case 2:
                    anim.Play("dronehit");
                    anim.speed = Random.Range(1f, 2f);
                    break;
                case 3:
                    anim.Play("GetHit");
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
        if(instantiatedIntent != null){
            Destroy(instantiatedIntent);
        }
        instantiatedIntent = Instantiate(intent[2], transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(0,0,0));
    }
    public void reducedamageby(float a, int b){
        weakentill = b;
        weakenvalue = a;
    }
    GameObject instantiatepoisonstack;
    public void addpoison(int a){
        poison += a;
        instantiatepoisonstack = Instantiate(poisonstack, debufftrans);
        instantiatepoisonstack.GetComponentInChildren<TMP_Text>().text = poison.ToString();
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
        if(instantiatedIntent != null){
            Destroy(instantiatedIntent);
        }
        if(poison > 0){
            if(instantiatepoisonstack!=null){
                instantiatepoisonstack.GetComponentInChildren<TMP_Text>().text = poison.ToString();
            }else{
                instantiatepoisonstack = Instantiate(poisonstack, debufftrans);
            }
            damaged(poison, poisoneffect);
            poison--;
        }
        if(stunned > 0){
            stunned--;
            instantiatedIntent = Instantiate(intent[2], transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(0,0,0));
            return;
        }
        mv = variation[Random.Range(0, variation.Count)];
        instantiatedIntent = Instantiate(intent[mv.type], transform.position + new Vector3(0, intentheight, 0), Quaternion.Euler(0,0,0));
        instantiatedIntent.GetComponentInChildren<TMP_Text>().text = (mv.value * weakenvalue).ToString();
    }

    public void move(){
        if(stunned > 0){
            return;
        }
        shieldvalue = 0;
        shieldtext.text = shieldvalue.ToString();
        shieldobject.SetActive(false);
        switch(mv.type){
            case 0:
                atk();
                break;
            case 1:
                def();
                break;
            case 3:
                buff();
                break;
        }
    }

    void atk(){
        soundcontroller.Instance.playsound(8);
        for(int i = 0; i<mv.multiplicative;i++){
            Debug.Log("sssss" + "  " + (int)(mv.value  * weakenvalue));
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

    void buff(){
        soundcontroller.Instance.playsound(8);
        for(int i = 0; i<mv.multiplicative;i++){
            foreach(movevariation m in variation){
                if(m.type == 3){
                    break;
                }
                m.value += mv.value;
            }
        }
    }
}
