using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class battlescript : MonoBehaviour
{
    public static battlescript Instance {get; private set;} = null;
 
    private void Awake(){
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public phaseClass[] phaseArray;
    public GameObject[] cardselectedlist, phaseObject, attackeffect;
    public Transform[] deckshows;
    public GameObject goToEffect, rewards, cardSelect, relic, shieldobject, canvas, damageshow, deathscreen, relicobject, healobject;
    public Transform arrow, rewardGrid, relicgrid;
    public int maxEnergy = 3, energy = 3, idx = 0;
    IDamageable enemyscript;
    public Slider healthbar;
    public TMP_Text energytxt, shieldtext, healthtext, healthtext1;
    public List<relicvariant> relicVariantList = new List<relicvariant>();
    public Animator anim;

    bool interactionOnGoing = false, candraw = false;
    
    int shieldvalue = 0, avoidattack = 0, nextturnshieldvalue = 0, nextturnenergyvalue = 0, nextturnhealvalue = 0, healduration = 0, reducedamagetakenduration = 0;

    public List<GameObject> enemylist = new List<GameObject>();

    float reducedamagetaken = 1;

    relicvariant relicinstantiated;

    void Start(){
        DontDestroyOnLoad(this.gameObject);
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        healthtext1.text = healthbar.value + " / " + healthbar.maxValue;
    }

    public void sceneChanger(string a){
        SceneManager.LoadScene(a);

        if(a.Equals("event")){
            //canvas.SetActive(false);
            Debug.Log("kita gk ada lagi scene quiz, jadi pop up");
        }else{
            interactionOnGoing = true;
            canvas.SetActive(true);
            soundcontroller.Instance.playsound(9);
            StartBattle();
        }
    }

    public void showEffect(int value, Vector3 pos){
        GameObject a = Instantiate(damageshow, pos, Quaternion.Euler(0,0,0));
        a.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = value.ToString();
        a.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0,4), ForceMode2D.Impulse);
        Destroy(a, 0.75f);
    }

    public void enableRewards(bool isrelic){
        rewards.SetActive(true);
        canvas.SetActive(false);
        if(isrelic == true){
            relic.SetActive(true);
            int relicchoosen = Random.Range(0, relicVariantList.Count);
            relicinstantiated = relicVariantList[relicchoosen];
            relic.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = relicinstantiated.relicname;
            relic.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = relicinstantiated.image;
        }else{
            cardSelect.SetActive(true);
        }
    }
    public void spawnrelic(){
        GameObject b = Instantiate(relicobject, relicgrid);
        reliclogic rl = b.GetComponent<reliclogic>();
        rl.setdata(relicinstantiated.image);
        getrelic(relicinstantiated.getRelicName);
    }

    IEnumerator wait(){
        yield return new WaitForSeconds(0.2f);
        trueStartTurn();
        yield return new WaitForSeconds(0.3f);
        GameObject a = GameObject.FindGameObjectWithTag("enemy");
        if(a != null){
            arrow.gameObject.SetActive(true);
            enemyscript = a.GetComponent<IDamageable>();
            arrow.position = a.transform.position + new Vector3(0,0,0);
        }else{
            arrow.gameObject.SetActive(false);
        }
    }

    public void StartBattle(){
        StartCoroutine(wait());
    }

    public bool reduceEnergy(int cardCost){
        if(energy>=cardCost){
            energy -= cardCost;
            energytxt.text = energy.ToString();
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
                    arrow.position = hit.collider.gameObject.transform.position + new Vector3(0,0,0);
                }
            }
        }
    }

    public void activatequiz(){
        questiongenerator.Instance.StartQuiz();
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

    public void killedanenemy(){
        GameObject a = GameObject.FindGameObjectWithTag("enemy");
        if(a != null){
            arrow.gameObject.SetActive(true);
            enemyscript = a.GetComponent<IDamageable>();
            arrow.position = a.transform.position + new Vector3(0,0,0);
        }else{
            arrow.gameObject.SetActive(false);
        }
        StartCoroutine(waitfindenemy());
    }
    IEnumerator waitfindenemy(){
        yield return new WaitForSeconds(0.6f);
        if(enemyscript==null){
            GameObject a = GameObject.FindGameObjectWithTag("enemy");
            if(a != null){
                arrow.gameObject.SetActive(true);
                enemyscript = a.GetComponent<IDamageable>();
                arrow.position = a.transform.position + new Vector3(0,0,0);
            }else{
                arrow.gameObject.SetActive(false);
            }
        }
    }

    public bool checkInteractionOnGoing(){
        return interactionOnGoing;
    }

    public void changePhaseMinuss(){
        foreach(GameObject a in phaseObject){
            a.SetActive(false);
        }
        idx--;
        if(idx<0){
            idx = 2;
        }
        phaseObject[idx].SetActive(true);
        startTurn(false);
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
        startTurn(false);
    }

    public void trueEndTurn(){
        if(interactionOnGoing == false){
            interactionOnGoing = true;
            candraw = true;
            StartCoroutine(trueEndTurnCoroutine());
            soundcontroller.Instance.playsound(5);
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
    }

    IEnumerator trueEndTurnCoroutine(){
        interactionOnGoing = true;
        List<IDamageable> enemyInstantiatedList = GameObject.Find("enemyspawner").GetComponent<enemyspawner>().enemyInstantiatedList;
        if(enemyInstantiatedList.Count == 0){
            yield break;
        }
        soundcontroller.Instance.playsound(5);
        foreach(IDamageable a in enemyInstantiatedList){
            enemyInstantiatedList.RemoveAll(item => item == null);
            yield return new WaitForSeconds(0.5f);
            if(a.checkstillalive()==true){
                a.move();
            }
        }
        foreach(IDamageable a in enemyInstantiatedList){
            yield return new WaitForSeconds(0.2f);
            enemyInstantiatedList.RemoveAll(item => item == null);
            if(a.checkstillalive()==true){
                a.prepareattack();
            }
        }

        trueStartTurn();
    }

    public void trueStartTurn(){
        killedanenemy();
        if(nextturnshieldvalue > 0){
            shieldvalue = nextturnshieldvalue;
            shieldtext.text = shieldvalue.ToString();
            shieldobject.SetActive(true);
        }else{
            shieldvalue = 0;
            shieldtext.text = shieldvalue.ToString();
            shieldobject.SetActive(false);
        }

        if(nextturnhealvalue > 0){
            if(healduration > 0){
                healthbar.value += nextturnhealvalue;
                healduration--;
            }else{
                nextturnhealvalue = 0;
            }
        }
        nextturnshieldvalue = 0;

        refreshEnergy();
        interactionOnGoing = false;
        startTurn(true);
    }

    void refreshEnergy(){
        energy = maxEnergy;
        energytxt.text = energy.ToString();
        soundcontroller.Instance.playsound(5);
        interactionOnGoing = false;
    }

    public void discardtodrawpile(){
        while(phaseArray[idx].discardpile.Count > 0){
            RectTransform a = phaseArray[idx].discardpile[0];
            phaseArray[idx].drawpile.Add(a);
            phaseArray[idx].discardpile.Remove(a);
            a.SetParent(phaseArray[idx].drawpileTrans, false);
        }
    }

    public void startTurn(bool startofround){
        if(startofround == false){
            return;
        }
        Debug.Log("DRAWS");
        for(int i = 0; i < phaseArray.Length; i++){
            for(int j = 0; j < 3; j++){
                if(phaseArray[i].drawpile.Count <= 0){
                    if(phaseArray[i].discardpile.Count > 0){
                        discardtodrawpile();
                    }else{
                        break;
                    }
                }
                RectTransform b = phaseArray[i].drawpile[Random.Range(0, phaseArray[i].drawpile.Count)];
                b.gameObject.GetComponent<cardlogic>().turnRaycast(true);
                phaseArray[i].drawpile.Remove(b);
                phaseArray[i].cardInHandList.Add(b);
                b.SetParent(phaseArray[i].handTrans, false);
                b.sizeDelta = new Vector2 (100, 125);
                b.localScale = new Vector3(1,1,1);
                b.anchorMax = new Vector2 (0.5f, 0.5f);
                b.anchorMin = new Vector2 (0.5f, 0.5f);
                StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
            }
        }
    }

    public void endTurn(bool isforward){
        soundcontroller.Instance.playsound(5);
        if(isforward == true){
            changePhase();
        }else{
            changePhaseMinuss();
        }
    }

    IEnumerator goToEffectAnim(Vector3 start, Vector3 destination){
        Transform a = Instantiate(goToEffect).transform;
        a.position = start;
        while(Vector3.Distance(a.position, destination) > 0.5f){
            a.position = Vector3.MoveTowards(a.position, destination, 1);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void getupgrade(){
        
    }
    public void gethealrestsite(){
        soundcontroller.Instance.playsound(6);
        healthbar.value += healthbar.value * 0.3f;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        healthtext1.text = healthbar.value + " / " + healthbar.maxValue;
        healobject.SetActive(true);
    }
    public void getheal(int heal){
        soundcontroller.Instance.playsound(6);
        healthbar.value += heal;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        healthtext1.text = healthbar.value + " / " + healthbar.maxValue;
        healobject.SetActive(true);
    }
    public void damaged(int damage){
        if(avoidattack > 0){
            avoidattack--;
            return;
        }
        anim.Play("getHit");
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
        if(reducedamagetakenduration > 0){
            damage = (int)(damage * reducedamagetaken);
            reducedamagetakenduration--;
        }
        healthbar.value -= damage;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        healthtext1.text = healthbar.value + " / " + healthbar.maxValue;
        battlescript.Instance.showEffect(damage, transform.position);

        if(healthbar.value <= 0){
            dead();
        }
    }
    public void removehealth(int damage){
        if(avoidattack > 0){
            avoidattack--;
            return;
        }
        anim.Play("getHit");
        healthbar.value -= damage;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
        healthtext1.text = healthbar.value + " / " + healthbar.maxValue;
        battlescript.Instance.showEffect(damage, transform.position);

        if(healthbar.value <= 0){
            dead();
        }
    }
    public void dead(){
        deathscreen.SetActive(true);
    }
    public void endrun(){
        sceneChanger("Main menu");
        Destroy(this.gameObject);
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
        GameObject b = null;
        switch(a.GetComponent<cardlogic>().type){
            case cardtype.effect:
                b = Instantiate(a, phaseArray[0].drawpileTrans);
                phaseArray[0].drawpile.Add(b.GetComponent<RectTransform>());
                break;
            case cardtype.function:
                b = Instantiate(a, phaseArray[1].drawpileTrans);
                phaseArray[1].drawpile.Add(b.GetComponent<RectTransform>());
                break;
            case cardtype.variable:
                b = Instantiate(a, phaseArray[2].drawpileTrans);
                phaseArray[2].drawpile.Add(b.GetComponent<RectTransform>());
                break;
        }

        switch(a.GetComponent<cardlogic>().type){
            case cardtype.effect:
                Instantiate(a, deckshows[0]).GetComponent<cardlogic>().enabled = false;
                break;
            case cardtype.function:
                Instantiate(a, deckshows[1]).GetComponent<cardlogic>().enabled = false;
                break;
            case cardtype.variable:
                Instantiate(a, deckshows[2]).GetComponent<cardlogic>().enabled = false;
                break;
        }
    }
    void getrelic(string a){
        Invoke(a, 0.1f);
    }
    public void getsword(){
        Debug.Log("do something");
    }
    public void gettriangle(){
        Debug.Log("do something");
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
        soundcontroller.Instance.playsound(0);
        shieldobject.SetActive(true);
        shieldvalue += value;
        shieldtext.text = shieldvalue.ToString();
    }

    public void mixingCard(){
        if((function.Equals("outputcast") || function.Equals("riskymove") || function.Equals("backwarddefense") || function.Equals("redraw") || function.Equals("clone")) && !effect.Equals("") && enemyscript!=null && interactionOnGoing == false){
            Invoke(function, 0.1f);
            Debug.Log(variable);
            for(int i = 0;i < cardselectedlist.Length; i++){
                if(i == 0){
                    continue;
                }
                RectTransform a = cardselectedlist[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                phaseArray[i].discardpile.Add(a);
                phaseArray[i].cardInHandList.Remove(a);
                a.SetParent(phaseArray[i].discardpileTrans, false);
                StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
            }
            return;
        }

        if(variable!=-1 && !function.Equals("") && !effect.Equals("") && enemyscript!=null && interactionOnGoing == false){
            Invoke(function, 0.1f);
            Debug.Log(variable);
            for(int i = 0;i < cardselectedlist.Length; i++){
                RectTransform a = cardselectedlist[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                phaseArray[i].discardpile.Add(a);
                phaseArray[i].cardInHandList.Remove(a);
                a.SetParent(phaseArray[i].discardpileTrans, false);
                StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
            }
        }
    }

    //============================================================= FUNCTION

    IEnumerator removeAllDelay(){
        interactionOnGoing = true;
        yield return new WaitForSeconds(0.5f);
        removeAll();
        interactionOnGoing = false;
    }

    void forloop(){
        for(int i = 0; i < variable; i++){
            Invoke(effect, 0.1f);
        }
        StartCoroutine(removeAllDelay());
    }

    void outputcast(){
        Invoke(effect, 0.1f);
        removeAll();
    }

    void switchcast(){
        switch(variable){
            case 1:
                Invoke("gigacyberattack", 0.1f);
                break;
            case 2:
                Invoke("continuousheal", 0.1f);
                break;
            case 3:
                Invoke("systemwarning", 0.1f);
                break;
        }
        removeAll();
    }

    void multiplier(){
        variable *= variable;
        Invoke(effect, 0);
        removeAll();
    }

    void recursive(){
        //do something
        removeAll();
    }

    void ifelse(){
        RectTransform a = null;
        if(variable > 3){
            foreach(phaseClass b in phaseArray){
                if(phaseArray[idx].drawpile.Count <= 0){
                    if(phaseArray[idx].discardpile.Count > 0){
                        discardtodrawpile();
                    }else{
                        break;
                    }
                }
                if(candraw == false){
                    break;
                }
                a = b.drawpile[Random.Range(0, b.drawpile.Count)];
                a.gameObject.GetComponent<cardlogic>().turnRaycast(true);
                b.drawpile.Remove(a);
                b.cardInHandList.Add(a);
                a.SetParent(b.handTrans, true);
            }
        }
        foreach(phaseClass b in phaseArray){
            if(phaseArray[idx].drawpile.Count <= 0){
                if(phaseArray[idx].discardpile.Count > 0){
                    discardtodrawpile();
                }else{
                    break;
                }
            }
            if(candraw == false){
                break;
            }
            a = b.drawpile[Random.Range(0, b.drawpile.Count)];
            a.gameObject.GetComponent<cardlogic>().turnRaycast(true);
            b.drawpile.Remove(a);
            b.cardInHandList.Add(a);
            a.SetParent(b.handTrans, true);
        }
        removeAll();
    }

    void multiply(){
        variable += variable * 2;
        Invoke(effect, 0);
        removeAll();
    }

    void riskymove(){
        variable *= 2;
        Invoke(effect, 0);
        damaged((int)(variable / 2));
        removeAll();
    }

    void backwarddefense(){
        variable += shieldvalue / 2;
        Invoke(effect, 0);
        removeAll();
    }

    void defensepreparation(){
        nextturnshieldvalue += 8 + variable;
        Invoke(effect, 0);
        removeAll();
    }

    void exponential(){
        int endvariable = variable;
        for(int j = 0; j<variable; j++){
            endvariable *= endvariable;
        }
        Invoke(effect, 0);
        removeAll();
    }

    void redraw(){
        for(int i =0; i < variable / 2; i++){
            energy++;
            energytxt.text = energy.ToString();
        }
        for(int i =0; i < variable; i++){
            foreach(phaseClass b in phaseArray){
                if(phaseArray[idx].drawpile.Count <= 0){
                    if(phaseArray[idx].discardpile.Count > 0){
                        discardtodrawpile();
                    }else{
                        break;
                    }
                }
                if(candraw == false){
                    break;
                }
                RectTransform a = b.drawpile[Random.Range(0, b.drawpile.Count)];
                a.gameObject.GetComponent<cardlogic>().turnRaycast(true);
                b.drawpile.Remove(a);
                b.cardInHandList.Add(a);
                a.SetParent(b.handTrans, true);
            }
        }
        candraw = false;
        removeAll();
    }

    void damagetoshield(){
        soundcontroller.Instance.playsound(3);
        enemyscript.damaged(variable, attackeffect[0]);
        getshield(variable);
        removeAll();
    }

    void weaken(){
        enemyscript.reducedamageby(0.2f, variable);
        removeAll();
    }

    void lifesteal(){
        getheal(enemyscript.stealhealth(variable));
        removeAll();
    }

    void stun(){
        enemyscript.stunfor(variable);
        removeAll();
    }

    void chargeup(){
        nextturnenergyvalue += 1 + variable;
        Invoke(effect, 0.1f);
        removeAll();
    }

    void clone(){
        avoidattack++;
        Invoke(effect, 0.1f);
        removeAll();
    }

    void recklessforloop(){
        for(int i = 0; i < variable; i++){
            Invoke(effect, 0.1f);
            damaged((int)(variable / 3));
        }
        removeAll();
    }

    void revengestrike(){
        enemyscript.damaged(variable * ((int)healthbar.maxValue - (int)healthbar.value), attackeffect[0]);
        removeAll();
    }

    void sacrificialstrike(){
        enemyscript.damaged(8 + variable, attackeffect[0]);
        removehealth(variable);
        removeAll();
    }

    //============================================================= OUTPUT / EFFECT

    void cyberattack(){
        soundcontroller.Instance.playsound(3);
        Debug.Log(variable);
        enemyscript.damaged(variable, attackeffect[0]);
    }

    void gigacyberattack(){
        soundcontroller.Instance.playsound(3);
        enemyscript.damaged(variable * 2, attackeffect[0]);
    }

    void heal(){
        getheal(variable);
    }

    void continuousheal(){
        nextturnhealvalue = variable;
        healduration = 3;
    }

    void shield(){
        getshield(variable);
    }

    void systemwarning(){
        enemyscript.stunfor(1);
        reducedamagetaken = 0.3f;
        reducedamagetakenduration = 2;
        variable = (int)(variable * 1.5f);
        cyberattack();
    }

    void poison(){
        enemyscript.addpoison(variable);
    }

    //============================================================= RELIC

    void getmaxhealthup(){
        healthbar.maxValue += 6;
        healthbar.value += 6;

    }
}

public enum cardtype{
    effect,
    function,
    variable
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

[System.Serializable]
public class relicvariant
{
    public Sprite image;
    public string getRelicName;
    public string relicname;
}