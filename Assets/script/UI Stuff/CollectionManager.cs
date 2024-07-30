using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    [SerializeField] private List<GameObject> cardList = new List<GameObject>();
    private List<CollectionItemHolder> cards = new List<CollectionItemHolder>();
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
        PopulateCardList(new string[] {"Assets/script/UI Stuff/Items/effect_collection",
                                        "Assets/script/UI Stuff/Items/function_collection",
                                        "Assets/script/UI Stuff/Items/variable_collection"});
        GetCardsFromList();
    }

    private void PopulateCardList(string[] paths)
    {
        cardList.Clear();
        foreach (string path in paths)
        {
            string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { path });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (obj != null)
                {
                    cardList.Add(obj);
                }
            }
        }
        Debug.Log("Effect card list populated successfully!");
    }

    private void GetCardsFromList()
    {
        foreach (var card in cardList)
        {
            CollectionItemHolder data = card.GetComponent<CollectionItemHolder>();
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

    public List<CollectionItemHolder> ReadAllCardData()
    {
        Debug.Log(cards.Count);
        return new List<CollectionItemHolder>(cards);
    }
}
