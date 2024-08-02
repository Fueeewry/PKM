using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text cardInfo;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject cardContent;
    [SerializeField] private GameObject cardButtonPrefab;

    private List<CollectionItemHolder> cards = new List<CollectionItemHolder>();
    private List<GameObject> selections = new List<GameObject>();

    private int selectedIndex = 0;

    [SerializeField] private TMP_Text tutorText;
    [SerializeField] private Image tutorImage;
    [SerializeField] private GameObject tutorContent;
    [SerializeField] private GameObject tutorButtonPrefab;
    [SerializeField] private RectTransform layoutGroupRectTransform;

    public List<TutorialContent> tutorials;

    private void OnEnable()
    {
        RefreshContent();
        InitializeTutorialContents();
    }

    private void RefreshContent()
    {
        cards.Clear();
        selections.Clear();
        foreach (Transform child in cardContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        cards = CollectionManager.Instance.ReadAllCardData();
        GenerateSelectionButton();

        ShowCardInfo(selectedIndex);
    }

    private void GenerateSelectionButton()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            int index = i;
            GameObject selection = Instantiate(cardButtonPrefab, cardContent.transform);
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
        selections[selectedIndex].transform.GetChild(0).gameObject.SetActive(false);
        selectedIndex = index;

        selections[index].transform.GetChild(0).gameObject.SetActive(true);

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

    public void ShowTutorialContent(int index)
    {
        //Debug.Log("showing at index " + index);
        if(tutorials[index].image == null)
        {
            tutorImage.gameObject.SetActive(false);
        }
        else
        {
            tutorImage.gameObject.SetActive(true);
            tutorImage.sprite = tutorials[index].image;
        }
        tutorText.text = tutorials[index].text;

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupRectTransform);
    }

    private void InitializeTutorialContents()
    {
        foreach (Transform child in tutorContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int count = tutorials.Count;
        for (int i = 0; i < count; i++)
        {
            int index = i;
            GameObject button = Instantiate(tutorButtonPrefab, tutorContent.transform);
            button.GetComponentInChildren<TMP_Text>().text = tutorials[index].title;
            button.GetComponent<Button>().onClick.AddListener(() => ShowTutorialContent(index));
        }

        ShowTutorialContent(0);
    }
}

[Serializable]
public class TutorialContent
{
    public string title;
    public Sprite image;
    public String text;

    public TutorialContent(String title, Sprite image, String text)
    {
        this.title = title;
        this.image = image;
        this.text = text;
    }
}
