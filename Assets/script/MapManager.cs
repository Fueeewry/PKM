using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public GameObject MapCanvas;

    [SerializeField] private Button mapButton;
    [SerializeField] private Sprite mapButtonEnabledImg;
    [SerializeField] private Sprite mapButtonDisabledImg;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        mapButton.onClick.AddListener(()=> MapButton());
        SetMapDisplay(false);
    }

    public void SetMapDisplay(bool boolean)
    {
        if(MapCanvas != null)
        {
            MapCanvas.SetActive(boolean);
        }
    }

    public void MapButtonActivate(bool boolean)
    {
        if (boolean)
        {
            mapButton.image.sprite = mapButtonEnabledImg;
            mapButton.image.raycastTarget = true;
        }
        else
        {
            mapButton.image.sprite = mapButtonDisabledImg;
            mapButton.image.raycastTarget = false;
        }
    }

    public void MapButton() // pasang ke button map
    {
        if (MapCanvas.activeSelf)
        {
            SetMapDisplay(false);
        }
        else
        {
            SetMapDisplay(true);
        }
    }
}
