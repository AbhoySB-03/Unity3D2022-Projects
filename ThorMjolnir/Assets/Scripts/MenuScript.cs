using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Dropdown resolutionField;
    [SerializeField] private Toggle fullScreen;
    [SerializeField] private Dropdown quality;
    private List<int[]> resolutionList=new List<int[]>();


    private void Awake()
    {
        LoadSettings();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadSettings()
    {
        Resolution curRes = Screen.currentResolution;
        int curValue = 0;
        resolutionField.ClearOptions();
        resolutionList.Clear();
        foreach (Resolution r in Screen.resolutions)
        {
            int[] res = new int[3];
            res[0]=r.width; res[1] = r.height; res[2] = r.refreshRate;
            resolutionList.Add(res);
        }

        List<string> opts= new List<string>();
        for (int i=0; i<resolutionList.Count; i++)
        {
            
           opts.Add(resolutionList[i][0] + " X " + resolutionList[i][1] + " " + resolutionList[i][2] + " Hz");
            
            if (resolutionList[i][0] == curRes.width && resolutionList[i][1] == curRes.height && resolutionList[i][2] == curRes.refreshRate)
            {
                curValue = i;
            }
        }

        resolutionField.AddOptions(opts);
        resolutionField.value = curValue;
        resolutionField.RefreshShownValue();


        List<string> qual= new List<string>();
        qual.Clear();
        foreach (string s in QualitySettings.names) { 
            qual.Add(s);
        }
        
        quality.ClearOptions();
        quality.AddOptions(qual);
        quality.value = QualitySettings.GetQualityLevel();
        quality.RefreshShownValue();
    }


    public void ApplyChanges()
    {
        int Width = resolutionList[resolutionField.value][0];
        int Height = resolutionList[resolutionField.value][1];
        int RefRate = resolutionList[resolutionField.value][2];

        Screen.SetResolution(Width, Height, fullScreen.isOn, RefRate);
        QualitySettings.SetQualityLevel(quality.value, true);


                
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
