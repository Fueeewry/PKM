using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardInfo;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject buttonPrefab;

    private List<CollectionItemHolder> cards = new List<CollectionItemHolder>();
    private List<GameObject> selections = new List<GameObject>();

    private void OnEnable()
    {
        RefreshContent();
    }

    private void RefreshContent()
    {
        cards.Clear();
        selections.Clear();
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        cards = CollectionManager.Instance.ReadAllCardData();
        GenerateSelectionButton();

        if(cardImage.sprite == null)
        {
            ShowCardInfo(0);
        }
    }

    private void GenerateSelectionButton()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            int index = i;
            GameObject selection = Instantiate(buttonPrefab, content.transform);
            selections.Add(selection);

            Image selectionImage = selection.GetComponent<Image>();
            Button button = selectionImage.GetComponent<Button>();
            if(selectionImage != null)
            {
                selectionImage.sprite = cards[i].cardSprite;
            }
            if(button != null)
            {
                button.onClick.AddListener(() => ShowCardInfo(index));
            }
        }
    }

    private void ShowCardInfo(int index)
    {
        cardImage.sprite = cards[index].cardSprite;
        string infoText = GenerateInfoText(index);
        cardInfo.text = infoText;
    }

    private string GenerateInfoText(int index)
    {
        string text = "";
        text += $"Type : {cards[index].cardLogic.type}\n";
        text += $"Cost : {cards[index].cardLogic.cost}\n";
        switch (cards[index].cardLogic.type)
        {
            case cardtype.effect:
                text += $"Effect : {cards[index].cardLogic.effect}\n";
                break;
            case cardtype.function:
                text += $"Function : {cards[index].cardLogic.function}\n";
                break;
            case cardtype.variable:
                text += $"Variable : {cards[index].cardLogic.variable}\n";
                break;
        }
        return text;
    }
}
