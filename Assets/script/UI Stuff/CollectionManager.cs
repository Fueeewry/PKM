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
        PopulateCardList(new string[] {"Collection Items/effect_collection",
                                        "Collection Items/function_collection",
                                        "Collection Items/variable_collection"});
        GetCardsFromList();
    }

    private void PopulateCardList(string[] paths)
    {
        cardList.Clear();
        foreach (string path in paths)
        {
            GameObject[] objects = Resources.LoadAll<GameObject>(path);
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    cardList.Add(obj);
                }
            }
        }
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
        return new List<CollectionItemHolder>(cards);
    }
}
