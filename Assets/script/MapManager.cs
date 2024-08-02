using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
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

    public Material lineMaterial;

    [ColorUsage(false, true)] public List<Color> lineColors = new List<Color>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        mapButton.onClick.AddListener(()=> MapButton());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1) && MapCanvas.activeSelf == true) MaterialColorRandomizer();
    }

    public void SetMapDisplay(bool boolean)
    {
        if(MapCanvas != null)
        {
            MapCanvas.SetActive(boolean);
        }
        if(boolean == true)
        {
            MaterialColorRandomizer();
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

    public void MaterialColorRandomizer()
    {
        int random = 0;
        do
        {
            random = UnityEngine.Random.Range(0, lineColors.Count);
        } 
        while (lineMaterial.color == lineColors[random]);


        StartCoroutine(ColorLerp(random));

        
        //lineMaterial.color = lineColors[random];
    }

    public IEnumerator ColorLerp(int random)
    {
        for (int i = 0; i < 100; i++)
        {
            lineMaterial.color = Color.Lerp(lineMaterial.color, lineColors[random], 0.2f);
            yield return new WaitForSeconds(0.02f);
        }
        
    }
}
