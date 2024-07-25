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
    public GameObject[] cardselectedlist, phaseObject;
    public Transform[] deckshows;
    public Stack<GameObject> energyObjectList = new Stack<GameObject>();
    public GameObject goToEffect, energyObject, rewards, cardSelect, relic, shieldobject, canvas, damageshow, deathscreen, relicobject;
    public Transform arrow, energySlotTrans, rewardGrid, relicgrid;
    public int maxEnergy = 3, energy = 3, idx = 0;
    IDamageable enemyscript;
    public Slider healthbar;
    public TMP_Text energytxt, shieldtext, healthtext;
    public List<relicvariant> relicVariantList = new List<relicvariant>();
    public Animator anim;

    bool interactionOnGoing = false;

    int shieldvalue = 0, avoidattack = 0, nextturnshieldvalue = 0, nextturnenergyvalue = 0;

    public List<GameObject> enemylist = new List<GameObject>();

    relicvariant relicinstantiated;

    void Start(){
        DontDestroyOnLoad(this.gameObject);
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
    }

    public void sceneChanger(string a){
        SceneManager.LoadScene(a);

        if(a.Equals("event")){
            canvas.SetActive(false);
        }else{
            interactionOnGoing = true;
            canvas.SetActive(true);
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
        startTurn();
        StartCoroutine(refreshEnergy());
        yield return new WaitForSeconds(0.2f);
        GameObject a = GameObject.FindGameObjectWithTag("enemy");
        if(a != null){
            arrow.gameObject.SetActive(true);
            enemyscript = a.GetComponent<IDamageable>();
            arrow.position = a.transform.position + new Vector3(0,3,0);
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
            if(energyObjectList.Count > 0){
                Destroy(energyObjectList.Pop());
            }
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

    public void killedanenemy(){
        GameObject a = GameObject.FindGameObjectWithTag("enemy");
        if(a != null){
            arrow.gameObject.SetActive(true);
            enemyscript = a.GetComponent<IDamageable>();
            arrow.position = a.transform.position + new Vector3(0,3,0);
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
                arrow.position = a.transform.position + new Vector3(0,3,0);
            }else{
                arrow.gameObject.SetActive(false);
            }
        }
    }

    public bool checkInteractionOnGoing(){
        return interactionOnGoing;
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
        interactionOnGoing = true;
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

    IEnumerator trueEndTurnCoroutine(){
        List<IDamageable> enemyInstantiatedList = GameObject.Find("enemyspawner").GetComponent<enemyspawner>().enemyInstantiatedList;
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
        if(nextturnshieldvalue >= 0){
            shieldvalue = nextturnshieldvalue;
            shieldtext.text = shieldvalue.ToString();
            shieldobject.SetActive(true);
        }else{
            shieldvalue = 0;
            shieldtext.text = shieldvalue.ToString();
            shieldobject.SetActive(false);
        }
        nextturnshieldvalue = 0;
        StartCoroutine(refreshEnergy());
    }

    IEnumerator refreshEnergy(){
        energy = maxEnergy;
        energytxt.text = energy.ToString();
        energyObjectList.Clear();
        soundcontroller.Instance.playsound(5);
        for(int i = maxEnergy-1; i>=0; i--){
            yield return new WaitForSeconds(0.05f);
            GameObject a = Instantiate(energyObject, energySlotTrans);
            energyObjectList.Push(a);
            if(i==maxEnergy-1 || i == 0){
                a.transform.localPosition += new Vector3((maxEnergy-1-i) * 40, i * 40, 0);
            }else{
                a.transform.localPosition += new Vector3((maxEnergy-1-i) * 55, i * 55, 0);
            }
        }
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

    public void startTurn(){
        for(int i = 0; i < 5; i++){
            if(phaseArray[idx].drawpile.Count <= 0){
                discardtodrawpile();
            }
            RectTransform b = phaseArray[idx].drawpile[Random.Range(0, phaseArray[idx].drawpile.Count)];
            b.gameObject.GetComponent<cardlogic>().turnRaycast(true);
            phaseArray[idx].drawpile.Remove(b);
            phaseArray[idx].cardInHandList.Add(b);
            b.SetParent(phaseArray[idx].handTrans, false);
            b.sizeDelta = new Vector2 (100, 125);
            b.localScale = new Vector3(1,1,1);
            b.anchorMax = new Vector2 (0.5f, 0.5f);
            b.anchorMin = new Vector2 (0.5f, 0.5f);
            StartCoroutine(goToEffectAnim(phaseArray[idx].drawpileTrans.position, phaseArray[idx].handTrans.position));
        }
    }

    public void endTurn(){
        soundcontroller.Instance.playsound(5);
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

    public void getupgrade(){
        
    }
    public void gethealrestsite(){
        soundcontroller.Instance.playsound(6);
        healthbar.value += healthbar.value * 0.3f;
    }
    public void getheal(int heal){
        soundcontroller.Instance.playsound(6);
        healthbar.value += heal;
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
                return;
            }
        }
        healthbar.value -= damage;
        healthtext.text = healthbar.value + " / " + healthbar.maxValue;
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
        switch(a.GetComponent<cardlogic>().type){
            case cardtype.effect:
                Instantiate(a, phaseArray[0].drawpileTrans);
                break;
            case cardtype.function:
                Instantiate(a, phaseArray[1].drawpileTrans);
                break;
            case cardtype.variable:
                Instantiate(a, phaseArray[2].drawpileTrans);
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
        if(variable!=-1 && !function.Equals("") && !effect.Equals("") && enemyscript!=null){
            Invoke(function, 0.1f);

            for(int i = 0;i < cardselectedlist.Length; i++){
                RectTransform a = cardselectedlist[i].transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                phaseArray[i].discardpile.Add(a);
                phaseArray[i].cardInHandList.Remove(a);
                a.SetParent(phaseArray[i].discardpileTrans, false);
                StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
            }
        }

        if((function.Equals("riskymove")||function.Equals("backwarddefense")||function.Equals("clone")) && !effect.Equals("") && enemyscript!=null){
            Invoke(function, 0.1f);

            for(int i = 0;i < cardselectedlist.Length; i++){
                Transform b = cardselectedlist[i].transform.GetChild(0);
                if(b != null){
                    RectTransform a = b.gameObject.GetComponent<RectTransform>();
                    phaseArray[i].discardpile.Add(a);
                    phaseArray[i].cardInHandList.Remove(a);
                    a.SetParent(phaseArray[i].discardpileTrans, false);
                    StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
                }
            }
        }

        if(variable!=-1 && (function.Equals("damagetoshield")||function.Equals("weaken")||function.Equals("lifesteal")||function.Equals("stun")||function.Equals("revengestrike")) && enemyscript!=null){
            Invoke(function, 0.1f);

            for(int i = 0;i < cardselectedlist.Length; i++){
                Transform b = cardselectedlist[i].transform.GetChild(0);
                if(b != null){
                    RectTransform a = b.gameObject.GetComponent<RectTransform>();
                    phaseArray[i].discardpile.Add(a);
                    phaseArray[i].cardInHandList.Remove(a);
                    a.SetParent(phaseArray[i].discardpileTrans, false);
                    StartCoroutine(goToEffectAnim(phaseArray[i].drawpileTrans.position, phaseArray[i].handTrans.position));
                }
            }
        }
    }

    int[] cardvalue = {6, 3, 3, 3}; //HARUS SAMA
    static int[] staticcardvalue = {6, 3, 3, 3};

    //============================================================= FUNCTION

    void repeater(){
        for(int i = 0; i < variable; i++){
            Invoke(effect, 0.1f);
        }
        removeAll();
    }

    void outputcast(){
        Invoke(effect, 0.1f);
        removeAll();
    }

    void switchcast(){ //update dari sini
        switch(variable){
            case 1:
                Invoke("cyberattack", 0.1f);
                break;
            case 2:
                Invoke("heal", 0.1f);
                break;
            case 3:
                Invoke("shield", 0.1f);
                break;
        }
        removeAll();
    }

    void multiplier(){
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] *= variable;
        }
        Invoke(effect, 0);
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] = staticcardvalue[i];
        }
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
                if(b.drawpile.Count <= 0){
                    discardtodrawpile();
                }
                a = b.drawpile[Random.Range(0, b.drawpile.Count)];
                a.gameObject.GetComponent<cardlogic>().turnRaycast(true);
                b.drawpile.Remove(a);
                b.cardInHandList.Add(a);
                a.SetParent(b.handTrans, true);
            }
        }
        foreach(phaseClass b in phaseArray){
            if(b.drawpile.Count <= 0){
                discardtodrawpile();
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
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] += variable * 2;
        }
        Invoke(effect, 0);
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] = staticcardvalue[i];
        }
        removeAll();
    }

    void riskymove(){
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] *= 2;
        }
        Invoke(effect, 0);
        switch(effect){
            case "cyberattack":
                damaged(cardvalue[0] / 2);
                break;
            case "heal":
                damaged(cardvalue[1] / 2);
                break;
            case "shield":
                damaged(cardvalue[2] / 2);
                break;
            case "poison":
                damaged(cardvalue[3] / 2);
                break;
        }
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] = staticcardvalue[i];
        }
        removeAll();
    }

    void backwarddefense(){
        for(int i = 0; i<cardvalue.Length; i++){
            cardvalue[i] += shieldvalue / 2;
        }
        Invoke(effect, 0);
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] = staticcardvalue[i];
        }
        removeAll();
    }

    void defensepreparation(){
        nextturnshieldvalue += 8 + variable;
        Invoke(effect, 0);
        removeAll();
    }

    void exponential(){
        for(int i = 0; i<cardvalue.Length; i++){
            for(int j = 0; j<variable; j++){
                cardvalue[i] *= cardvalue[i];
            }
        }
        Invoke(effect, 0);
        for(int i = 0; i < cardvalue.Length; i++){
            cardvalue[i] = staticcardvalue[i];
        }
        removeAll();
    }

    void redraw(){
        for(int i =0; i < variable / 2; i++){
            GameObject a = Instantiate(energyObject, energySlotTrans);
            energyObjectList.Push(a);
            if(i==maxEnergy-1 || i == 0){
                a.transform.localPosition += new Vector3((maxEnergy-1-i) * 40, i * 40, 0);
            }else{
                a.transform.localPosition += new Vector3((maxEnergy-1-i) * 55, i * 55, 0);
            }
        }
        for(int i =0; i < variable; i++){
            foreach(phaseClass b in phaseArray){
                if(b.drawpile.Count <= 0){
                    discardtodrawpile();
                }
                RectTransform a = b.drawpile[Random.Range(0, b.drawpile.Count)];
                a.gameObject.GetComponent<cardlogic>().turnRaycast(true);
                b.drawpile.Remove(a);
                b.cardInHandList.Add(a);
                a.SetParent(b.handTrans, true);
            }
        }
        removeAll();
    }

    void damagetoshield(){
        soundcontroller.Instance.playsound(3);
        enemyscript.damaged(variable);
        getshield(variable);
        removeAll();
    }

    void weaken(){
        //enemyscript.reducedamageby(..., variable);
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
            switch(effect){
            case "cyberattack":
                damaged(cardvalue[0] / 3);
                break;
            case "heal":
                damaged(cardvalue[1] / 3);
                break;
            case "shield":
                damaged(cardvalue[2] / 3);
                break;
            case "poison":
                damaged(cardvalue[3] / 3);
                break;
            }
        }
        removeAll();
    }

    void revengestrike(){
        enemyscript.damaged(variable * ((int)healthbar.maxValue - (int)healthbar.value));
        removeAll();
    }

    void sacrificialstrike(){
        enemyscript.damaged(8 + variable);
        removehealth(variable);
        removeAll();
    }

    //============================================================= OUTPUT / EFFECT

    void cyberattack(){
        soundcontroller.Instance.playsound(3);
        enemyscript.damaged(cardvalue[0]);
    }

    void heal(){
        getheal(cardvalue[1]);
    }

    void shield(){
        getshield(cardvalue[2]);
    }

    void poison(){
        //enemyscript.addpoison(cardvalue[3]);
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