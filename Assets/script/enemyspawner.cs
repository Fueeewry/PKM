using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyspawner : MonoBehaviour
{
    public List<enemycombination> enemyCombinationList;
    public List<IDamageable> enemyInstantiatedList = new List<IDamageable>();
    void Start()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait(){
        yield return new WaitForSeconds(0.25f);
        List<GameObject> enemylist = FindObjectOfType<battlescript>().enemylist;
        int[] a = enemyCombinationList[Random.Range(0, enemyCombinationList.Count)].combination;
        for(int i = 0; i<a.Length;i++){
            GameObject b = Instantiate(enemylist[a[i]], transform.position + new Vector3((i + 2) * 2 , 0, 0), Quaternion.Euler(0,0,0));
            enemyInstantiatedList.Add(b.GetComponent<IDamageable>());
        }
        InvokeRepeating("checkbattledone", 0.1f, 0.1f);
    }

    public void checkbattledone(){
        enemyInstantiatedList.RemoveAll(item => item == null);
        if(enemyInstantiatedList.Count == 0){
            battlescript.Instance.enableRewards(false);
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
