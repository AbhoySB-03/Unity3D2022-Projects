using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private GameObject panel, settings, about;
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private AudioMixer mixer_audio;

    private enum PanelMode
    {
        none, settings, about
    }

    private PanelMode panelMode;
    // Start is called before the first frame update
    void Awake()
    {
        panelMode = PanelMode.none;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitSettings()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(QualitySettings.names.ToList<string>());
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        if (!mixer_audio.GetFloat("volume", out float vol))
            return;
        audioToggle.isOn = vol > -80f;

    }

    public void ApplySettings()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        mixer_audio.SetFloat("volume", audioToggle.isOn ? 0 : -80);
    }

    public void Settings()
    {
        if (panelMode!=PanelMode.settings)
        {
            panel.SetActive(true);
            settings.SetActive(true);
            about.SetActive(false);
            InitSettings();
            panelMode = PanelMode.settings;
            return;
        }
        panel.SetActive(false);
        panelMode = PanelMode.none;
    }

    public void About()
    {
        if (panelMode != PanelMode.about)
        {
            panel.SetActive(true);
            settings.SetActive(false);
            about.SetActive(true);
            panelMode = PanelMode.about;
            return;
        }
        panel.SetActive(false);
        panelMode = PanelMode.none;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
