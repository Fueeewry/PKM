using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyspawner : MonoBehaviour
{
    public static enemyspawner Instance {get; private set;} = null;

    private void Awake(){
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public List<enemycombination> enemyCombinationList;
    public List<IDamageable> enemyInstantiatedList = new List<IDamageable>();
    public GameObject bossobject, droneboss;
    public bool isboss = false;
    void Start()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait(){
        yield return new WaitForSeconds(0.25f);
        if(isboss == false){
            List<GameObject> enemylist = battlescript.Instance.enemylist;
            int[] a = enemyCombinationList[Random.Range(0, enemyCombinationList.Count)].combination;
            for(int i = 0; i<a.Length;i++){
                GameObject b = Instantiate(enemylist[a[i]], transform.position + new Vector3((i + 2) * 2 , (i + 2) * -1, 0), Quaternion.Euler(0,0,0));
                enemyInstantiatedList.Add(b.GetComponent<IDamageable>());
            }
            InvokeRepeating("checkbattledone", 0.1f, 0.1f);
        }else{
            int i = 0;
            GameObject b = Instantiate(droneboss, transform.position + new Vector3((i + 2) * 2 , 0, 0), Quaternion.Euler(0,0,0));
            enemyInstantiatedList.Add(b.GetComponent<IDamageable>());
            i++;
            b = Instantiate(bossobject, transform.position + new Vector3((i + 2) * 2 , 0, 0), Quaternion.Euler(0,0,0));
            enemyInstantiatedList.Add(b.GetComponent<IDamageable>());
            i++;
            b = Instantiate(droneboss, transform.position + new Vector3((i + 2) * 2 , 0, 0), Quaternion.Euler(0,0,0));
            enemyInstantiatedList.Add(b.GetComponent<IDamageable>());
            InvokeRepeating("checkbattledone", 0.1f, 0.1f);
        }
    }

    public void checkbattledone(){
        enemyInstantiatedList.RemoveAll(item => item == null);
        if(enemyInstantiatedList.Count == 0){
            battlescript.Instance.enableRewards(false);
            CancelInvoke("checkbattledone");
        }
    }
}

[System.Serializable]
public class enemycombination {
    public int[] combination;
    enemycombination(int[] combination){
        this.combination = combination;
    }
}
