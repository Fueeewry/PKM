using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class battlescript : MonoBehaviour
{
    public phaseClass[] phaseArray;
    public GameObject[] cardselectedlist, phaseObject;
    public Stack<GameObject> energyObjectList = new Stack<GameObject>();
    public GameObject goToEffect, energyObject, rewards, cardSelect, relic, shieldobject;
    public Transform arrow, energySlotTrans, rewardGrid;
    public int maxEnergy = 3, energy = 3, idx = 0;
    IDamageable enemyscript;
    public Slider healthbar;
    public TMP_Text energytxt, shieldtext;
    public List<relicvariant> relicVariantList = new List<relicvariant>();

    int shieldvalue = 0;

    public List<GameObject> enemylist = new List<GameObject>();
    relicvariant relicinstantiated;

    public void enableRewards(bool isrelic){
        rewards.SetActive(true);
        if(isrelic == true){
            relic.SetActive(true);
            relicinstantiated = relicVariantList[Random.Range(0, relicVariantList.Count)];
            relic.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = relicinstantiated.relicname;
            relic.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = relicinstantiated.image;
            //a.GetComponent<relicgetlogic>().getRelicName = 
        }else{
            cardSelect.SetActive(true);
        }
    }

    void OnEnable(){
        StartCoroutine(wait());

    }
    IEnumerator wait(){
        yield return new WaitForSeconds(0.2f);
        startTurn();
        StartCoroutine(refreshEnergy());
        yield return new WaitForSeconds(0.75f);
        GameObject a = GameObject.FindGameObjectWithTag("enemy");
        enemyscript = a.GetComponent<IDamageable>();
        arrow.position = a.transform.position + new Vector3(0,3,0);
    }

    public bool reduceEnergy(int cardCost){
        if(energy>=cardCost){
            energy -= cardCost;
            energytxt.text = energy.ToString();
            Destroy(energyObjectList.Pop());
        }else{
            return false;
        }
        return true;
    }

    void FixedUpdate()
    {
        radial();
    }
    void Update(){
        if(Input.GetButtonDown("Fire1")){
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
 
            if(hit.collider != null){
                if(hit.collider.gameObject.tag == "enemy"){
                    enemyscript = hit.collider.gameObject.GetComponent<IDamageable>();
                    arrow.position = hit.collider.gameObject.transform.position + new Vector3(0,3,0);
                }else if(hit.collider.gameObject.tag == "quiz"){
                    hit.collider.gameObject.GetComponent<questiongenerator>().StartQuiz();
                }
            }
        }
    }

    void radial(){
        phaseArray[idx].cardInHandList.RemoveAll(item => item == null);
        phaseArray[idx].drawpile.RemoveAll(item => item == null);
        phaseArray[idx].discardpile.RemoveAll(item => item == null);
        int r = (phaseArray[idx].cardInHandList.Count / 2 ), k = 0, plus = -1, notmidpoint = 1, r1 = -1;
        bool even = false;
        if(phaseArray[idx].cardInHandList.Count%2==0){
            even = true;
            r1 = r--;
        }
        for(int i = 0 ;i<phaseArray[idx].cardInHandList.Count;i++){
            if(even && i == r1){
                phaseArray[idx].cardInHandList[i].anchoredPosition = new Vector3((i - r) * 75,k*20 - 40,0);
                phaseArray[idx].cardInHandList[i].localRotation = Quaternion.Euler(0,0,0);
                continue;
            }
            if(i<r){
                k++;
                plus = -1;
                notmidpoint = 1;
            }else if(i>r){
                k--;
                plus = 1;
                notmidpoint = 1;
            }else if(i == r){
                k = r;
                k++;
                notmidpoint = 0;
            }
            phaseArray[idx].cardInHandList[i].anchoredPosition = new Vector3((i - r) * 75,k*20 - 40,0);
            phaseArray[idx].cardInHandList[i].localRotation = Quaternion.Euler(0,0,(k - phaseArray[idx].cardInHandList.Count) * 5 * plus * notmidpoint);
        }
    }

    public void changePhase(){
        foreach(GameObject a in phaseObject){
            a.SetActive(false);
        }
        idx++;
        if(idx>2){
            idx = 0;
        }
        phaseObject[idx].SetActive(true);
        startTurn();
    }

    public void trueEndTurn(){
        StartCoroutine(trueEndTurnCoroutine());
        for(int i = 0;i < cardselectedlist.Length; i++){
            if(cardselectedlist[i].transform.childCount > 1){
                RectTransform a = cardselectedlist[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>();
                phaseArray[i].discardpile.Add(a);
                phaseArray[i].cardInHandList.Remove(a);
                a.SetParent(phaseArray[i].discardpileTrans, false);
                StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
            }
        }
    }

    IEnumerator trueEndTurnCoroutine(){
        List<IDamageable> enemyInstantiatedList = GameObject.Find("enemyspawner").GetComponent<enemyspawner>().enemyInstantiatedList;
        foreach(IDamageable a in enemyInstantiatedList){
            yield return new WaitForSeconds(0.5f);
            if(a!=null){
                a.move();
            }
        }
        foreach(IDamageable a in enemyInstantiatedList){
            yield return new WaitForSeconds(0.2f);
            if(a!=null){
                a.prepareattack();
            }
        }

        trueStartTurn();
    }

    public void trueStartTurn(){
        shieldvalue = 0;
        shieldtext.text = shieldvalue.ToString();
        shieldobject.SetActive(false);
        StartCoroutine(refreshEnergy());
    }

    IEnumerator refreshEnergy(){
        energy = maxEnergy;
        energytxt.text = energy.ToString();
        energyObjectList.Clear();
        for(int i = maxEnergy-1; i>=0; i--){
            yield return new WaitForSeconds(0.2f);
            GameObject a = Instantiate(energyObject, energySlotTrans);
            energyObjectList.Push(a);
            if(i==maxEnergy-1 || i == 0){
                a.transform.localPosition += new Vector3((maxEnergy-1-i) * 40, i * 40, 0);
            }else{
                a.transform.localPosition += new Vector3((maxEnergy-1-i) * 55, i * 55, 0);
            }
        }
    }

    public void startTurn(){
        for(int i = 0; i < 5; i++){
            if(phaseArray[idx].drawpile.Count <= 0){
                if(phaseArray[idx].discardpile.Count <= 0){
                    break;
                }
                while(phaseArray[idx].discardpile.Count > 0){
                    RectTransform a = phaseArray[idx].discardpile[0];
                    phaseArray[idx].drawpile.Add(a);
                    phaseArray[idx].discardpile.Remove(a);
                    a.SetParent(phaseArray[idx].drawpileTrans, false);
                }
            }
            RectTransform b = phaseArray[idx].drawpile[Random.Range(0, phaseArray[idx].drawpile.Count)];
            b.gameObject.GetComponent<cardlogic>().turnRaycast(true);
            phaseArray[idx].drawpile.Remove(b);
            phaseArray[idx].cardInHandList.Add(b);
            b.SetParent(phaseArray[idx].handTrans, true);
            b.sizeDelta = new Vector2 (100, 125);
            b.localScale = new Vector3(1,1,1);
            b.anchorMax = new Vector2 (0.5f, 0.5f);
            b.anchorMin = new Vector2 (0.5f, 0.5f);
            StartCoroutine(goToEffectAnim(phaseArray[idx].drawpileTrans.position, phaseArray[idx].handTrans.position));
        }
    }

    public void endTurn(){
        foreach(phaseClass b in phaseArray){
            while(b.cardInHandList.Count > 0){
                RectTransform a = b.cardInHandList[0];
                b.discardpile.Add(a);
                b.cardInHandList.Remove(a);
                a.SetParent(b.discardpileTrans, false);
            }   
        }
        changePhase();
    }

    IEnumerator goToEffectAnim(Vector3 start, Vector3 destination){
        Transform a = Instantiate(goToEffect).transform;
        a.position = start;
        while(Vector3.Distance(a.position, destination) > 0.5f){
            a.position = Vector3.MoveTowards(a.position, destination, 1);
            yield return new WaitForSeconds(0.02f);
        }
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
    }

    public void reset(){
        foreach(phaseClass b in phaseArray){
            while(b.discardpile.Count > 0){
                RectTransform a = b.discardpile[0];
                b.drawpile.Add(a);
                b.discardpile.Remove(a);
                a.SetParent(b.drawpileTrans, false);
            }
            while(b.cardInHandList.Count > 0){
                RectTransform a = b.cardInHandList[0];
                b.discardpile.Add(a);
                b.cardInHandList.Remove(a);
                a.SetParent(b.drawpileTrans, false);
            }  
        }
    }

    public void getcard(GameObject a){
        Instantiate(a, phaseArray[idx].drawpileTrans);
    }
    public void getrelic(){
        Invoke(relicinstantiated.getRelicName, 0.1f);
    }

    public void removeCard(RectTransform a){
        phaseArray[idx].cardInHandList.Remove(a);
    }

    int variable = -1;
    string function = "", effect = "";

    public void addVariable(int x){
        variable = x;
    }

    public void addFunction(string function){
        this.function = function;
    }

    public void addEffect(string effect){
        this.effect = effect;
    }

    public void removeVariable(){
        variable = -1;
    }

    public void removeFunction(){
        this.function = "";
    }

    public void removeEffect(){
        this.effect = "";
    }

    public void removeAll(){
        removeVariable();
        removeFunction();
        removeEffect();
    }

    public void getshield(int value){
        shieldobject.SetActive(true);
        shieldvalue += value;
        shieldtext.text = shieldvalue.ToString();
    }

    public void mixingCard(){
        if(variable!=-1 && !function.Equals("") && !effect.Equals("") && enemyscript!=null){
            Invoke(function, 0.1f);

            for(int i = 0;i < cardselectedlist.Length; i++){
                RectTransform a = cardselectedlist[i].transform.GetChild(1).gameObject.GetComponent<RectTransform>();
                phaseArray[i].discardpile.Add(a);
                phaseArray[i].cardInHandList.Remove(a);
                a.SetParent(phaseArray[i].discardpileTrans, false);
                StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
            }
        }
    }

    //============================================================= FUNCTION

    public void repeater(){
        for(int i = 0; i < variable; i++){
            Invoke(effect, 0.1f);
        }
        removeAll();
    }

    //============================================================= OUTPUT / EFFECT

    public void fireball(){
        enemyscript.damaged(6);
    }

    //============================================================= RELIC

    public void getmaxhealthup(){
        healthbar.maxValue += 6;
        healthbar.value += 6;
    }
}

[System.Serializable]
public class movevariation{
    public int value;
    public int type;
    public int multiplicative = 1;

    public movevariation(int value, int type, int multiplicative){
        this.value = value;
        this.type = type;
        this.multiplicative = multiplicative;
    }   
}