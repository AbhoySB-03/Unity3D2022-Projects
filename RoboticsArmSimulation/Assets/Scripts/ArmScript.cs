using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Transform armBase;
    [SerializeField] private GameObject[] disableAtDisassembly;
    [SerializeField] private GameObject HelpDisplay;

    private float startBaseAngle;
    private ArmPartScript armBasePart;
    private ArmPartScript[] armParts;
    // Start is called before the first frame update
    void Start()
    {
        armParts= GetComponentsInChildren<ArmPartScript>();
        armBasePart=armBase.GetComponent<ArmPartScript>();
    }

    // Update is called once per frame
    public void AssembleParts()
    {
        foreach (GameObject g in disableAtDisassembly)
        {
            g.SetActive(true);
        }
        for (int i = 0; i < armParts.Length; i++)
        {
            armParts[i].Assemble();
        }
    }

    public void DisassembleParts()
    {
        for (int i=0; i<armParts.Length; i++)
        {
            armParts[i].Disassemble();
        }

        foreach (GameObject g in disableAtDisassembly)
        {
            g.SetActive(false);
        }
    }

    public void CloseSim()
    {
        Application.Quit();
    }

    public void ShowHelp()
    {
        HelpDisplay.SetActive(!HelpDisplay.activeSelf);
    }

    
}
