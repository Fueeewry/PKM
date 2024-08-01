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
