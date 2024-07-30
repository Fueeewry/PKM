using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Linq;


public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    int currentResolutionIndex = 0;

    private void Awake()
    {
        if(audioMixer == null)
        {
            audioMixer = GetComponent<AudioMixer>();
        }
    }

    private void Start()
    {
        resolutions = new Resolution[]
        {
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 854, height = 480 }
        };
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SortResolutionArray()
    {
        var sortedResolutions = resolutions.OrderByDescending(r => r.width).ThenByDescending(r => r.refreshRateRatio).ToArray();
        resolutions = sortedResolutions;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}