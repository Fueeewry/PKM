using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    [SerializeField] private List<GameObject> cardList = new List<GameObject>();
    private List<cardlogic> cards = new List<cardlogic>();
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        GetCardsFromList();
    }

    private void GetCardsFromList()
    {
        foreach(var card in cardList)
        {
            cardlogic data = card.GetComponent<cardlogic>();
            if (data != null)
            {
                cards.Add(data);
            }
            else
            {
                Debug.LogWarning("Cannot find cardlogic component. Please check card list again.");
            }
        }
    }

    public List<cardlogic> ReadAllCardData()
    {
        Debug.Log(cards.Count);
        return new List<cardlogic>(cards);
    }
}
