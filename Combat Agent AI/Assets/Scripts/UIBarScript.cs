using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarScript : MonoBehaviour
{
    public HealthScript h;
    public GameObject hbar, sbar;
    Vector3 MaxScaleH, MaxScaleS;
    void Start()
    {
        MaxScaleH = hbar.transform.localScale;
        MaxScaleS = sbar.transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        
        hbar.transform.localScale = new Vector3(Mathf.Clamp01(h.health / h.MaxHealth) * MaxScaleH.x, MaxScaleH.y, MaxScaleH.z);
        sbar.transform.localScale = new Vector3(Mathf.Clamp01(h.combatstamina / h.MaxCombatStamina) * MaxScaleS.x, MaxScaleS.y, MaxScaleS.z);
    }
}
